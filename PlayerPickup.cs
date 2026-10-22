using UnityEngine;
using Unity.Netcode;

public class PlayerPickup : NetworkBehaviour
{
    public Transform holdPoint;      // Drag HoldPoint here in Inspector
    public float pickupRange = 5f;   // Pickup distance

    private NetworkObject heldObject;

    void Update()
    {
        // Only local player can pick
        if (!IsOwner) return;

        // Press E to pick object
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }

        // Keep object visible in hand
        if (heldObject != null)
        {
            heldObject.transform.position = holdPoint.position;
            heldObject.transform.rotation = holdPoint.rotation;

            // Fix scale issue
            heldObject.transform.localScale = Vector3.one;
        }
    }

    void TryPickup()
    {
        // Already holding something
        if (heldObject != null) return;

        GameObject[] pickableObjects = GameObject.FindGameObjectsWithTag("Pickable");

        float closestDistance = Mathf.Infinity;
        GameObject closestObject = null;

        foreach (GameObject obj in pickableObjects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);

            if (distance < closestDistance && distance <= pickupRange)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

        if (closestObject != null)
        {
            NetworkObject netObj = closestObject.GetComponent<NetworkObject>();

            if (netObj != null)
            {
                heldObject = netObj;
                PickServerRpc(netObj.NetworkObjectId);
            }
        }
        else
        {
            Debug.Log("No pickable object nearby");
        }
    }

    [ServerRpc]
    void PickServerRpc(ulong objectId)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(objectId))
            return;

        NetworkObject obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // Keep object visible
        Renderer rend = obj.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.enabled = true;
        }

        Debug.Log("Picked: " + obj.name);
    }
}