using Unity.Netcode;
using UnityEngine;

public class PickableBase : NetworkBehaviour
{
    public NetworkVariable<bool> isAvailable = new NetworkVariable<bool>(true);

    public override void OnNetworkSpawn()
    {
        ApplyAvailabilityState(isAvailable.Value);

        isAvailable.OnValueChanged += (oldValue, newValue) =>
        {
            ApplyAvailabilityState(newValue);
        };
    }

    protected virtual void ApplyAvailabilityState(bool newValue)
    {
        gameObject.SetActive(newValue);
    }
}