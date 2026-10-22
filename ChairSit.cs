using UnityEngine;

public class ChairSit : MonoBehaviour
{
    public Transform sitPoint;

    private GameObject currentPlayer;
    private Animator animator;
    private CharacterController cc;
    private PlayerControllers movement;

    private bool isSitting = false;

    private static ChairSit activeChair;

    void Update()
    {
        if (activeChair == this &&
            currentPlayer != null &&
            !isSitting &&
            Input.GetKeyDown(KeyCode.Return))
        {
            SitPlayer();
        }

        if (isSitting &&
            Input.GetKeyDown(KeyCode.Space))
        {
            StandPlayer();
        }
    }

    void SitPlayer()
    {
        cc = currentPlayer.GetComponent<CharacterController>();
        movement = currentPlayer.GetComponent<PlayerControllers>();
        animator = currentPlayer.GetComponentInChildren<Animator>();

        if (movement != null)
            movement.enabled = false;

        if (cc != null)
            cc.enabled = false;

        currentPlayer.transform.position = sitPoint.position;
        currentPlayer.transform.rotation = sitPoint.rotation;

        if (animator != null)
        {
            animator.SetBool("Running", false);
            animator.SetBool("Sitting", true);
        }

        if (cc != null)
            cc.enabled = true;

        isSitting = true;
    }

    void StandPlayer()
    {
        if (currentPlayer == null) return;

        movement = currentPlayer.GetComponent<PlayerControllers>();
        animator = currentPlayer.GetComponentInChildren<Animator>();
        cc = currentPlayer.GetComponent<CharacterController>();

        if (animator != null)
        {
            animator.SetBool("Sitting", false);

            // 🔥 FORCE RESET (VERY IMPORTANT)
            animator.CrossFade("Idle", 0.1f);
        }

        if (movement != null)
            movement.enabled = true;

        currentPlayer.transform.position +=
            currentPlayer.transform.forward * 1.2f;

        isSitting = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeChair = this;
            currentPlayer = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isSitting)
        {
            if (activeChair == this)
                activeChair = null;

            currentPlayer = null;
        }
    }
}