using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private bool initialized = false;

    void Start()
    {
        if (initialized) return;
        initialized = true;

        Debug.Log("GameScene Loaded");

        var nm = NetworkManager.Singleton;

        if (nm == null)
        {
            Debug.LogError("NetworkManager missing!");
            return;
        }

        var transport = nm.GetComponent<UnityTransport>();

        if (transport == null)
        {
            Debug.LogError("UnityTransport missing!");
            return;
        }

        transport.ConnectionData.Address = "127.0.0.1";
        transport.ConnectionData.Port = 8888;

        string role = PlayerPrefs.GetString("role", "Host");

        if (nm.IsListening)
        {
            Debug.Log("Netcode already running");
            return;
        }

        if (role == "Host")
        {
            StartCoroutine(StartHostSafe());
        }
        else
        {
            nm.StartClient();
        }
    }

    IEnumerator StartHostSafe()
    {
        yield return new WaitForSeconds(1f);

        var nm = NetworkManager.Singleton;

        if (nm != null && !nm.IsListening)
        {
            Debug.Log("Starting Host...");
            nm.StartHost();
        }
    }

    private void OnApplicationQuit()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}