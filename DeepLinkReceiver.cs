using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class DeepLinkReceiver : MonoBehaviour
{
    void Start()
    {
        string deepLink = GetDeepLink();

        if (!string.IsNullOrEmpty(deepLink))
        {
            Debug.Log("URL: " + deepLink);

            string user = GetParam(deepLink, "user");
            string token = GetParam(deepLink, "token");

            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(token))
            {
                PlayerPrefs.SetString("username", user);
                PlayerPrefs.SetString("token", token);

                StartCoroutine(Verify(user, token));
            }
            else
            {
                Debug.LogError("Missing data");
                SceneManager.LoadScene("LoginScene");
            }
        }
        else
        {
            Debug.Log("No deep link → manual login");
            SceneManager.LoadScene("LoginScene");
        }
    }

    string GetDeepLink()
    {
        string[] args = Environment.GetCommandLineArgs();

        foreach (string arg in args)
        {
            if (arg.StartsWith("unityapp://"))
                return arg;
        }

        return null;
    }

    IEnumerator Verify(string user, string token)
    {
        string url = "http://127.0.0.1:5000/api/verify_token";

        string json = "{\"username\":\"" + user + "\",\"token\":\"" + token + "\"}";

        UnityWebRequest req = new UnityWebRequest(url, "POST");

        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string res = req.downloadHandler.text;

            Debug.Log("SERVER: " + res);

            if (res.Contains("success"))
            {
                Debug.Log("✅ AUTH SUCCESS");

                // 👉 GO TO LOBBY (NOT GAME DIRECTLY)
                SceneManager.LoadScene("LobbyScene");
            }
            else
            {
                Debug.LogError("❌ AUTH FAIL");
                SceneManager.LoadScene("LoginScene");
            }
        }
        else
        {
            Debug.LogError("❌ NETWORK ERROR");
            SceneManager.LoadScene("LoginScene");
        }
    }

    string GetParam(string url, string key)
    {
        if (!url.Contains("?")) return "";

        string query = url.Split('?')[1];
        string[] parts = query.Split('&');

        foreach (string p in parts)
        {
            if (p.StartsWith(key + "="))
                return Uri.UnescapeDataString(p.Split('=')[1]);
        }

        return "";
    }
}