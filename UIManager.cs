using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject hostButton;
    public GameObject clientButton;

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (RoleManager.Instance.isHost)
        {
            hostButton.SetActive(true);    // Only host sees this
            clientButton.SetActive(false); // Optional: hide client for host
        }
        else
        {
            hostButton.SetActive(false);   // Friend will NOT see host button
            clientButton.SetActive(true);  // Only client button visible
        }
    }

    public void OnClickHost()
    {
        RoleManager.Instance.SetHost();
        UpdateUI();
        Debug.Log("Host selected");
    }

    public void OnClickClient()
    {
        RoleManager.Instance.SetClient();
        UpdateUI();
        Debug.Log("Client selected");
    }
}