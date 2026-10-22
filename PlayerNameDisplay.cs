using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class PlayerNameDisplay : NetworkBehaviour
{
    public TMP_Text nameText;
    public GameObject nameUI;

    public NetworkVariable<FixedString32Bytes> playerName =
        new NetworkVariable<FixedString32Bytes>(
            "",
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += OnNameChanged;

        UpdateName();

        // Hide ONLY your own name
        if (IsOwner && nameUI != null)
        {
            nameUI.SetActive(false);
        }
    }

    public override void OnNetworkDespawn()
    {
        playerName.OnValueChanged -= OnNameChanged;
    }

    private void OnNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        UpdateName();
    }

    private void UpdateName()
    {
        if (nameText != null)
        {
            nameText.text = playerName.Value.ToString();
        }
    }

    [ServerRpc]
    public void SetPlayerNameServerRpc(string newName)
    {
        playerName.Value = newName;
    }
}