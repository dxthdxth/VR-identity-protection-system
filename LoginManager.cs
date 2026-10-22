using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Text;
using TMPro;

[System.Serializable]
public class LoginRequestData
{
    public string username;
    public string password;
}

[System.Serializable]
public class LoginResponse
{
    public string status;
    public string username;
    public string token;
    public string message;
}

public class LoginManager : MonoBehaviour
{
    [Header("API")]
    [SerializeField] private string serverURL =
        "https://refract-waving-familiar.ngrok-free.dev/login";

    [Header("UI")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;

    [Header("Scene")]
    [SerializeField] private string lobbySceneName = "LobbyScene";

    private bool isLoggingIn = false;

    public void Login()
    {
        if (isLoggingIn) return;
        StartCoroutine(LoginRequest());
    }

    private IEnumerator LoginRequest()
    {
        isLoggingIn = true;

        string inputUsername = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(inputUsername) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Empty username/password");
            isLoggingIn = false;
            yield break;
        }

        LoginRequestData data = new LoginRequestData
        {
            username = inputUsername,
            password = password
        };

        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest request =
            new UnityWebRequest(serverURL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Network Error: " + request.error);
                isLoggingIn = false;
                yield break;
            }

            LoginResponse res =
                JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

            if (res == null || res.status != "success")
            {
                Debug.LogError("Login Failed: " + (res != null ? res.message : "Invalid response"));
                isLoggingIn = false;
                yield break;
            }

            string finalUsername =
                string.IsNullOrEmpty(res.username) ? inputUsername : res.username;

            string finalToken =
                string.IsNullOrEmpty(res.token) ? "" : res.token;

            // SAVE (IMPORTANT FOR MULTIPLAYER)
            PlayerPrefs.SetString("username", finalUsername);
            PlayerPrefs.SetString("token", finalToken);
            PlayerPrefs.Save();

            Debug.Log("Login Success: " + finalUsername);

            yield return new WaitForEndOfFrame();

            if (!Application.CanStreamedLevelBeLoaded(lobbySceneName))
            {
                Debug.LogError("Scene not in Build Settings");
                isLoggingIn = false;
                yield break;
            }

            SceneManager.LoadScene(lobbySceneName);
        }

        isLoggingIn = false;
    }
}