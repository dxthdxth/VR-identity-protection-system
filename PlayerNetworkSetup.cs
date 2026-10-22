using UnityEngine;
using Unity.Netcode;

public class PlayerNetworkSetup : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        Debug.Log("Player Spawned");

        if (IsOwner)
        {
            string username = PlayerPrefs.GetString("username", "Player");

            Debug.Log("Sending Username: " + username);

            PlayerNameDisplay nameDisplay =
                GetComponent<PlayerNameDisplay>();

            if (nameDisplay != null)
            {
                nameDisplay.SetPlayerNameServerRpc(username);
            }
            else
            {
                Debug.LogError("PlayerNameDisplay missing on Player prefab!");
            }
        }
    }
}