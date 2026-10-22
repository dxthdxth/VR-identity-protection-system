using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Text;
using TMPro;

[System.Serializable]
public class JoinRequest
{
    public string meeting_id;
}

[System.Serializable]
public class JoinResponse
{
    public string status;
    public string message;
    public string role;
}

public class JoinMeetingButton : MonoBehaviour
{
    public TMP_InputField meetingInput;
    public TMP_Text messageText;

    private bool isJoining = false;

    private string serverURL =
        "https://refract-waving-familiar.ngrok-free.dev/api/join_meeting";

    public void OnClick_JoinMeeting()
    {
        if (isJoining) return; // 🔥 prevent spam clicks
        StartCoroutine(JoinMeeting());
    }

    IEnumerator JoinMeeting()
    {
        isJoining = true;

        string meetingID = meetingInput.text.Trim();

        if (string.IsNullOrEmpty(meetingID))
        {
            messageText.text = "Enter Meeting ID";
            isJoining = false;
            yield break;
        }

        messageText.text = "Joining...";

        JoinRequest data = new JoinRequest
        {
            meeting_id = meetingID
        };

        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest req =
            new UnityWebRequest(serverURL, "POST"))
        {
            req.uploadHandler =
                new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));

            req.downloadHandler =
                new DownloadHandlerBuffer();

            req.SetRequestHeader("Content-Type", "application/json");

            req.timeout = 20;

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("JOIN ERROR: " + req.error);
                messageText.text = "Server Error";
                isJoining = false;
                yield break;
            }

            string responseText = req.downloadHandler.text;

            if (string.IsNullOrEmpty(responseText))
            {
                messageText.text = "Empty Response";
                isJoining = false;
                yield break;
            }

            JoinResponse res =
                JsonUtility.FromJson<JoinResponse>(responseText);

            if (res == null)
            {
                messageText.text = "Invalid Response";
                isJoining = false;
                yield break;
            }

            if (res.status == "success")
            {
                PlayerPrefs.SetString("meeting_id", meetingID);

                // 🔥 IMPORTANT: safe default
                PlayerPrefs.SetString("role", res.role ?? "Client");

                messageText.text = "Joining Game...";

                yield return new WaitForSeconds(0.5f);

                LoadGame();
            }
            else
            {
                messageText.text = res.message;
            }

            isJoining = false;
        }
    }

    void LoadGame()
    {
        if (Application.CanStreamedLevelBeLoaded("GameScene"))
        {
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogError("GameScene not in Build Settings!");
        }
    }
}