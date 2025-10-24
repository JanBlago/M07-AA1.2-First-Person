using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementNew : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float jumpForce = 5f;
    public float airControlMultiplier = 0.5f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    [Header("Sprint Settings")]
    public float sprintMultiplier = 2f;

    private Rigidbody rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool isGrounded;

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
        isGrounded = CheckGrounded();
        Move();
    }

    bool CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundLayer);
    }

    void Move()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move.y = 0f;

        // Velocidad horizontal
        float speedMultiplier = 1f;

        if (move.magnitude >= 0.1f) // Solo modificar speedMultiplier si hay input
        {
            // Velocidad según dirección
            if (moveInput.y < 0) speedMultiplier = 0.5f;                     // Atrás
            else if (Mathf.Abs(moveInput.x) > 0 && moveInput.y == 0) speedMultiplier = 0.75f; // Lados
            else speedMultiplier = 1f;                                       // Adelante o diagonal

            // Sprint
            if (isGrounded && Keyboard.current.leftShiftKey.isPressed && moveInput.y > 0)
                speedMultiplier *= sprintMultiplier;
        }

        // Aire
        if (!isGrounded) speedMultiplier *= airControlMultiplier;

        // Aplicar velocidad horizontal (mantener Y)
        Vector3 velocity = move * walkSpeed * speedMultiplier;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;

        // Salto
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Reinicia velocidad vertical
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            jumpPressed = false;
        }
    }


    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + Vector3.down * groundCheckDistance);
    }
}
