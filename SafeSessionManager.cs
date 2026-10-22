using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Text;

[System.Serializable]
public class SessionVerifyResponse
{
    public string status;
}

public class SafeSessionManager : MonoBehaviour
{
    private string verifyURL =
        "https://refract-waving-familiar.ngrok-free.dev/api/verify_token";

    private bool checking = false;
    private bool sessionReady = false;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(SessionLoop());
    }

    IEnumerator SessionLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            if (!checking)
            {
                yield return CheckSession();
            }
        }
    }

    IEnumerator CheckSession()
    {
        checking = true;

        string username = PlayerPrefs.GetString("username", "");
        string token = PlayerPrefs.GetString("token", "");

        Debug.Log($"SESSION CHECK -> USER: {username}, TOKEN: {token}");

        // ===============================
        // 1. WAIT UNTIL LOGIN EXISTS
        // ===============================
        if (string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(token))
        {
            if (!sessionReady)
            {
                Debug.Log("⏳ Waiting for login session...");
            }

            sessionReady = false;
            checking = false;
            yield break;
        }

        sessionReady = true;

        // ===============================
        // 2. BUILD REQUEST
        // ===============================
        string json =
            "{\"username\":\"" + username +
            "\",\"token\":\"" + token + "\"}";

        using (UnityWebRequest req = new UnityWebRequest(verifyURL, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.timeout = 10;

            yield return req.SendWebRequest();

            string responseText = req.downloadHandler.text;
            Debug.Log("VERIFY RESPONSE: " + responseText);

            // ===============================
            // 3. NETWORK ERROR HANDLING
            // ===============================
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("⚠ Network error: " + req.error);
                checking = false;
                yield break;
            }

            // ===============================
            // 4. FLASK AUTH FAIL (401)
            // ===============================
            if (req.responseCode == 401)
            {
                Debug.LogError("❌ Session expired (401)");

                ClearSession();
                SceneManager.LoadScene("LoginScene");

                checking = false;
                yield break;
            }

            // ===============================
            // 5. PARSE RESPONSE
            // ===============================
            SessionVerifyResponse res =
                JsonUtility.FromJson<SessionVerifyResponse>(responseText);

            if (res != null && res.status == "success")
            {
                Debug.Log("✅ Session valid");
            }
            else
            {
                Debug.LogError("❌ Invalid session response");

                ClearSession();
                SceneManager.LoadScene("LoginScene");
            }

            checking = false;
        }
    }

    void ClearSession()
    {
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.Save();
    }
}