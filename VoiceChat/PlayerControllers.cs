using UnityEngine;
using Unity.Netcode;

public class PlayerControllers : NetworkBehaviour
{
    private CharacterController controller;
    private Animator animator;

    public float speed = 5f;

    // Gravity
    public float gravity = -9.81f;
    private Vector3 velocity;

    public override void OnNetworkSpawn()
    {
        controller = GetComponent<CharacterController>();

        animator = GetComponentInChildren<Animator>();

        if (IsOwner && Camera.main != null)
        {
            CameraFollow cam =
                Camera.main.GetComponent<CameraFollow>();

            if (cam != null)
                cam.target = transform;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        // Ground check
        if (controller.isGrounded &&
            velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float h =
            Input.GetAxisRaw("Horizontal");

        float v =
            Input.GetAxisRaw("Vertical");

        Vector3 move =
            new Vector3(h, 0, v);

        if (move.magnitude > 0.1f)
        {
            move.Normalize();

            transform.forward = move;

            controller.Move(
                move *
                speed *
                Time.deltaTime
            );

            animator.SetBool("Running", true);
        }
        else
        {
            // IMPORTANT FIX
            if (!animator.GetBool("Sitting"))
            {
                animator.SetBool("Running", false);
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(
            velocity * Time.deltaTime
        );
    }
}