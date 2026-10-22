using UnityEngine;
using Unity.Netcode;

public class PlayerFileTarget : NetworkBehaviour
{
    private void OnMouseDown()
    {
        // Ignore clicking yourself
        if (IsOwner)
            return;

        Debug.Log(
            "Clicked Player ID: " +
            OwnerClientId
        );

        // Set target player
        FileTransferManager.Instance
            .SetTargetPlayer(
                OwnerClientId
            );
    }
}