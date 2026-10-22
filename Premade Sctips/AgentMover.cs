using UnityEngine;
public class AgentMover : MonoBehaviour
{
    [SerializeField]
    private CharacterController m_characterController;
    [SerializeField]
    private Animator m_animator;

    [SerializeField]
    private float m_movementSpeed = 4f;
    [SerializeField]
    private float m_rotationSpeed = 200f;
    public void Move(Vector2 movementInput)
    {
        transform.Rotate(Vector3.up, movementInput.x * Time.deltaTime * m_rotationSpeed);
        Vector3 direction = m_characterController.transform.forward;
        m_characterController
            .Move(direction * movementInput.y * m_movementSpeed * Time.deltaTime);
        
        m_animator.SetFloat("Movement", Mathf.Abs(movementInput.y));
    }
}
