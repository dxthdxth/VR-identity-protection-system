using UnityEngine;
using UnityEngine.SceneManagement;

public class DeepLinkManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Application.deepLinkActivated += OnDeepLinkActivated;

        // If app opened from browser
        if (!string.IsNullOrEmpty(Application.absoluteURL))
        {
            OnDeepLinkActivated(Application.absoluteURL);
        }
    }

    void OnDeepLinkActivated(string url)
    {
        Debug.Log("Deep Link Received: " + url);

        // Example:
        // unityapp://login

        if (url.Contains("login"))
        {
            Debug.Log("Loading Lobby Scene...");
            SceneManager.LoadScene("Lobby");
        }
    }
}