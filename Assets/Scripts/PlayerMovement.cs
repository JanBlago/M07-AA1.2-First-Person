using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementNew : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    private Rigidbody rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool jumpPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Player.Jump.performed += ctx => jumpPressed = true;
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        Move();
    }

    bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundLayer);
    }

    void Move()
    {
        if (IsGrounded())
        {
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            move.y = 0f;

            Vector3 velocity = move * speed;
            velocity.y = rb.linearVelocity.y;
            rb.linearVelocity = velocity;

            if (jumpPressed)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
                jumpPressed = false;
            }
        }
    }
}
