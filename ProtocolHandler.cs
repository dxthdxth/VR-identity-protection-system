using UnityEngine;
using UnityEngine.Networking;

public class ProtocolHandler : MonoBehaviour
{
    void Awake()
    {
        string[] args = System.Environment.GetCommandLineArgs();

        if (args == null || args.Length < 2)
        {
            Debug.LogWarning("No protocol URL received");
            return;
        }

        string url = args[1];

        Debug.Log("FULL URL: " + url);

        if (!url.StartsWith("unityapp://"))
        {
            Debug.LogWarning("Invalid protocol: " + url);
            return;
        }

        HandleURL(url);
    }

    void HandleURL(string url)
    {
        string username = "";
        string token = "";

        if (url.Contains("?"))
        {
            string query = url.Split('?')[1];
            string[] parts = query.Split('&');

            foreach (string part in parts)
            {
                if (part.StartsWith("user="))
                {
                    username = UnityWebRequest.UnEscapeURL(
                        part.Replace("user=", "")
                    ).Trim().ToLower(); // 🔥 FIX
                }

                if (part.StartsWith("token="))
                {
                    token = UnityWebRequest.UnEscapeURL(
                        part.Replace("token=", "")
                    ).Trim(); // 🔥 FIX
                }
            }
        }

        Debug.Log("PARSED USER: " + username);
        Debug.Log("PARSED TOKEN: " + token);

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
        {
            Debug.LogError("Invalid protocol data");
            return;
        }

        PlayerPrefs.SetString("username", username);
        PlayerPrefs.SetString("token", token);
        PlayerPrefs.Save();

        Debug.Log("Protocol login saved successfully");
    }
}