using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[System.Serializable]
public class AccessRequest
{
    public string username;
    public string token;
}

[System.Serializable]
public class AccessResponse
{
    public string status;
    public string message;
}

public class AccessManager : MonoBehaviour
{
    [SerializeField] private string serverURL =
        "https://refract-waving-familiar.ngrok-free.dev/api/verify_token";

    [SerializeField] private TMP_Text messageText;

    private void Start()
    {
        StartCoroutine(CheckAccess());
    }

    private IEnumerator CheckAccess()
    {
        string username = PlayerPrefs.GetString("username", "");
        string token = PlayerPrefs.GetString("token", "");

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
        {
            SetText("Session Missing");
            yield break;
        }

        AccessRequest data = new AccessRequest
        {
            username = username,
            token = token
        };

        string json = JsonUtility.ToJson(data);
        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest(serverURL, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();

            req.SetRequestHeader("Content-Type", "application/json");
            req.timeout = 30;

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                SetText("Server Error");
                yield break;
            }

            AccessResponse res =
                JsonUtility.FromJson<AccessResponse>(req.downloadHandler.text);

            if (res != null && res.status == "fail")
            {
                SetText("Session Expired");

                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();

                SceneManager.LoadScene("LoginScene");
            }
            else
            {
                SetText("Connected");
            }
        }
    }

    void SetText(string msg)
    {
        if (messageText != null)
            messageText.text = msg;
    }
}