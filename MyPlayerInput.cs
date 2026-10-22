using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayerInput : NetworkBehaviour
{
    [SerializeField]
    private InputActionReference m_movementReference;

    public Vector2 MovementInput { get; private set; }

    public event Action OnPickUpPressed;
    public event Action OnInteractPressed;

    private Vector2 m_rawInput;

    [SerializeField]
    private float m_smoothTime = 0.1f;

    private void OnEnable()
    {
        if (m_movementReference != null)
        {
            m_movementReference.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (m_movementReference != null)
        {
            m_movementReference.action.Disable();
        }
    }

    void Update()
    {
        // ✅ Only local player reads input
        if (!IsOwner)
            return;

        if (m_movementReference != null)
        {
            m_rawInput = m_movementReference.action.ReadValue<Vector2>();
            MovementInput = Vector2.MoveTowards(
                MovementInput,
                m_rawInput,
                Time.deltaTime / m_smoothTime
            );
        }

        // ✅ Keyboard input
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            OnPickUpPressed?.Invoke();
        }

        // ✅ Mouse input
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnInteractPressed?.Invoke();
        }
    }
}