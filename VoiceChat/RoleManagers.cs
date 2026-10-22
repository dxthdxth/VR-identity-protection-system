using UnityEngine;

public class RoleManager : MonoBehaviour
{
    public static RoleManager Instance;

    public bool isHost;

    private void Awake()
    {
        Instance = this;
    }

    public void SetHost()
    {
        isHost = true;
    }

    public void SetClient()
    {
        isHost = false;
    }
}