using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Text;

[System.Serializable]
public class ResponseData
{
    public string status;
    public string meeting_id;
    public string role;
    public string message;
}

[System.Serializable]
public class CreateRequest
{
    public string username;
    public string token;
}

[System.Serializable]
public class JoinRequestData
{
    public string meeting_id;
}

public class LobbyManager : MonoBehaviour
{
    private string serverURL =
        "https://refract-waving-familiar.ngrok-free.dev/api";

    public TMP_InputField meetingInputField;
    public TMP_Text messageText;
    public TMP_Text meetingIDText;

    private bool isBusy;

    void Start()
    {
        Debug.Log("User: " + SessionData.username);
        Debug.Log("Token: " + SessionData.token);
    }

    // ================= CREATE =================
    public void CreateMeeting()
    {
        if (!isBusy)
            StartCoroutine(CreateFlow());
    }

    IEnumerator CreateFlow()
    {
        isBusy = true;
        messageText.text = "Creating...";

        CreateRequest data = new CreateRequest
        {
            username = SessionData.username,
            token = SessionData.token
        };

        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest req =
            new UnityWebRequest(serverURL + "/create_meeting", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                messageText.text = "Network Error";
                isBusy = false;
                yield break;
            }

            ResponseData res =
                JsonUtility.FromJson<ResponseData>(req.downloadHandler.text);

            if (res != null && res.status == "success")
            {
                SessionData.meetingId = res.meeting_id;
                SessionData.role = "Host";

                messageText.text = "Created";

                yield return null;

                SceneManager.LoadScene("GameScene");
            }
            else
            {
                messageText.text = res?.message ?? "Create Failed";
            }
        }

        isBusy = false;
    }

    // ================= JOIN =================
    public void JoinMeeting()
    {
        if (!isBusy)
            StartCoroutine(JoinFlow());
    }

    IEnumerator JoinFlow()
    {
        isBusy = true;

        string meetingID = meetingInputField.text.Trim();

        if (string.IsNullOrEmpty(meetingID))
        {
            messageText.text = "Enter Meeting ID";
            isBusy = false;
            yield break;
        }

        messageText.text = "Joining...";

        JoinRequestData data = new JoinRequestData
        {
            meeting_id = meetingID
        };

        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest req =
            new UnityWebRequest(serverURL + "/join_meeting", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                messageText.text = "Network Error";
                isBusy = false;
                yield break;
            }

            ResponseData res =
                JsonUtility.FromJson<ResponseData>(req.downloadHandler.text);

            if (res != null && res.status == "success")
            {
                SessionData.meetingId = meetingID;
                SessionData.role = "Client";

                messageText.text = "Joined";

                yield return null;

                SceneManager.LoadScene("GameScene");
            }
            else
            {
                messageText.text = res?.message ?? "Join Failed";
            }
        }

        isBusy = false;
    }
}