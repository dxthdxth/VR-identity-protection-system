using UnityEngine;
using Unity.Netcode;

public class PickableTool : PickableBase
{
    private Collider objectCollider;
    private Renderer objectRenderer;
    private Rigidbody objectRigidbody;

    private void Awake()
    {
        // Get collider from main object
        objectCollider = GetComponent<Collider>();

        // IMPORTANT: get renderer from child objects too
        objectRenderer = GetComponentInChildren<Renderer>();

        // Get rigidbody
        objectRigidbody = GetComponent<Rigidbody>();
    }

    protected override void ApplyAvailabilityState(bool newValue)
    {
        if (newValue)
        {
            // Available on ground

            if (objectCollider != null)
                objectCollider.enabled = true;

            if (objectRenderer != null)
                objectRenderer.enabled = true;

            if (objectRigidbody != null)
            {
                objectRigidbody.useGravity = true;
                objectRigidbody.isKinematic = false;
            }
        }
        else
        {
            // Picked by player

            // Disable collider so no one else picks it
            if (objectCollider != null)
                objectCollider.enabled = false;

            // KEEP renderer ON so object is visible in hand
            if (objectRenderer != null)
                objectRenderer.enabled = true;

            if (objectRigidbody != null)
            {
                objectRigidbody.useGravity = false;
                objectRigidbody.isKinematic = true;
            }
        }
    }
}