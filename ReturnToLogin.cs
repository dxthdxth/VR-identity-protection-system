using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToLogin : MonoBehaviour
{
    public void GoToLogin()
    {
        SceneManager.LoadScene("LoginScene");
    }
}