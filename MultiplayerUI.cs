using UnityEngine;
using UnityEngine.UIElements;
using System;

public class MultiplayerUI : MonoBehaviour
{
    public UIDocument uiDocument;

    public Action OnStartHost;
    public Action OnStartClient;
    public Action OnDisconnectClient;

    Button hostButton;
    Button clientButton;
    Button disconnectButton;

    void Start()
    {
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument missing!");
            return;
        }

        var root = uiDocument.rootVisualElement;

        if (root == null)
        {
            Debug.LogError("Root Visual Element missing!");
            return;
        }

        hostButton = root.Q<Button>("HostButton");
        clientButton = root.Q<Button>("ClientButton");
        disconnectButton = root.Q<Button>("DisconnectButton");

        if (hostButton != null)
            hostButton.clicked += () => OnStartHost?.Invoke();

        if (clientButton != null)
            clientButton.clicked += () => OnStartClient?.Invoke();

        if (disconnectButton != null)
            disconnectButton.clicked += () => OnDisconnectClient?.Invoke();
    }

    public void DisableButtons()
    {
        if (hostButton != null) hostButton.SetEnabled(false);
        if (clientButton != null) clientButton.SetEnabled(false);
    }

    public void EnableButtons()
    {
        if (hostButton != null) hostButton.SetEnabled(true);
        if (clientButton != null) clientButton.SetEnabled(true);
    }
}