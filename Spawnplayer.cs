using UnityEngine;
using Unity.Netcode;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform[] spawnPoints;

    private bool callbackAdded = false;

    private void Start()
    {
        if (NetworkManager.Singleton == null)
            return;

        if (callbackAdded)
            return;

        callbackAdded = true;

        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayerForClient;
    }

    private void SpawnPlayerForClient(ulong clientId)
    {
        // Server only
        if (!NetworkManager.Singleton.IsServer)
            return;

        // 🔥 VERY IMPORTANT
        // Prevent duplicate player creation
        if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject != null)
        {
            Debug.Log("Player already exists for client: " + clientId);
            return;
        }

        Transform spawnPoint = null;

        if (spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[
                Random.Range(0, spawnPoints.Length)
            ];
        }

        Vector3 pos = spawnPoint != null
            ? spawnPoint.position
            : Vector3.zero;

        Quaternion rot = spawnPoint != null
            ? spawnPoint.rotation
            : Quaternion.identity;

        GameObject player = Instantiate(
            playerPrefab,
            pos,
            rot
        );

        player.GetComponent<NetworkObject>()
            .SpawnAsPlayerObject(clientId, true);

        Debug.Log("Spawned player for client: " + clientId);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayerForClient;
        }
    }
}