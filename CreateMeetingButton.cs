using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

[System.Serializable]
public class CreateResponse
{
    public string status;
    public string meeting_id;
    public string message;
}

public class CreateMeetingButton : MonoBehaviour
{
    public TMP_Text messageText;
    public TMP_Text meetingIDText;

    private bool isCreating = false;

    private string serverURL =
        "https://refract-waving-familiar.ngrok-free.dev/api/create_meeting";

    public void OnClick_CreateMeeting()
    {
        if (isCreating) return;
        StartCoroutine(CreateMeeting());
    }

    IEnumerator CreateMeeting()
    {
        isCreating = true;

        string username = PlayerPrefs.GetString("username", "");
        string token = PlayerPrefs.GetString("token", "");

        // 🔥 SAFE LOGIN CHECK
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
        {
            messageText.text = "Login Required";
            isCreating = false;
            yield break;
        }

        messageText.text = "Creating Meeting...";

        // 🔥 SAFE JSON (no string concat issues)
        CreateRequest data = new CreateRequest
        {
            username = username,
            token = token
        };

        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest req = new UnityWebRequest(serverURL, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            req.timeout = 20;

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(req.error);
                messageText.text = "Network Error";
                isCreating = false;
                yield break;
            }

            string responseText = req.downloadHandler.text;

            if (string.IsNullOrEmpty(responseText))
            {
                messageText.text = "Empty Response";
                isCreating = false;
                yield break;
            }

            CreateResponse res =
                JsonUtility.FromJson<CreateResponse>(responseText);

            if (res != null && res.status == "success")
            {
                PlayerPrefs.SetString("meeting_id", res.meeting_id);
                PlayerPrefs.SetString("role", "Host");

                if (meetingIDText != null)
                    meetingIDText.text = "Meeting ID: " + res.meeting_id;

                messageText.text = "Meeting Created";

                // 🔥 safer delay
                yield return new WaitForSeconds(1f);

                LoadGame();
            }
            else
            {
                messageText.text = res != null ? res.message : "Create Failed";
            }

            isCreating = false;
        }
    }

    // 🔥 FIXED REQUEST CLASS
    [System.Serializable]
    public class CreateRequest
    {
        public string username;
        public string token;
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