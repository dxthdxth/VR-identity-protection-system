using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private CharacterController controller;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        if (move.magnitude > 0.1f)
        {
            controller.Move(move * speed * Time.deltaTime);

            transform.forward = move;

            animator.SetBool("Running", true);
            animator.SetFloat("Speed", move.magnitude);
        }
        else
        {
            animator.SetBool("Running", false);
            animator.SetFloat("Speed", 0f);
        }
    }
}