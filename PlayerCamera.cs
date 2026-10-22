using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    public Camera cam;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            cam.enabled = false;
        }
    }
}