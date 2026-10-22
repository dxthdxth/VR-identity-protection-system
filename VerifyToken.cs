using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[System.Serializable]
public class VerifyTokenResponse
{
    public string status;
    public string message;
}

public class VerifyToken : MonoBehaviour
{
    private string verifyURL =
        "https://refract-waving-familiar.ngrok-free.dev/api/verify_token";

    public TMP_Text statusText;

    private bool isChecking = false;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(VerifyLoop());
    }

    IEnumerator VerifyLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            if (!isChecking)
                StartCoroutine(Verify());
        }
    }

    IEnumerator Verify()
    {
        isChecking = true;

        string username = PlayerPrefs.GetString("username", "");
        string token = PlayerPrefs.GetString("token", "");

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
        {
            SetText("Session Missing");
            isChecking = false;
            yield break;
        }

        // ✔ Correct JSON
        string json = JsonUtility.ToJson(new
        {
            username = username,
            token = token
        });

        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest(verifyURL, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();

            req.SetRequestHeader("Content-Type", "application/json");
            req.timeout = 30;

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                SetText("Server Error");
                isChecking = false;
                yield break;
            }

            VerifyTokenResponse res =
                JsonUtility.FromJson<VerifyTokenResponse>(req.downloadHandler.text);

            if (res != null && res.status == "fail")
            {
                SetText("Session Expired ❌");

                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();

                SceneManager.LoadScene("LoginScene");
            }
            else
            {
                SetText("Connected ✅");
            }
        }

        isChecking = false;
    }

    void SetText(string msg)
    {
        if (statusText != null)
            statusText.text = msg;
    }
}