using UnityEngine;
using UnityEngine.InputSystem;

public class PickupSystem : MonoBehaviour
{
    public float pickupRange = 3f;
    public float holdDistance = 2f;
    public float moveSpeed = 10f;

    public float minDistance = 1f;
    public float maxDistance = 4f;

    private Camera cam;
    private Rigidbody heldRb;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleScroll();

        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryPickup();

        if (Mouse.current.leftButton.wasReleasedThisFrame)
            Drop();

        if (heldRb != null)
            MoveObject();
    }

    // --------------------------------------------
    // 1. PICKUP QUE CHOCA CON EL MUNDO
    // --------------------------------------------
    void MoveObject()
    {
        Vector3 targetPos = cam.transform.position + cam.transform.forward * holdDistance;

        // MovePosition respeta colisiones
        heldRb.MovePosition(Vector3.Lerp(heldRb.position, targetPos, Time.deltaTime * moveSpeed));
    }

    // --------------------------------------------
    // 2. SCROLL PARA ACERCAR/ALEJAR
    // --------------------------------------------
    void HandleScroll()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) > 0.1f)
        {
            holdDistance -= scroll * 0.1f;
            holdDistance = Mathf.Clamp(holdDistance, minDistance, maxDistance);
        }
    }

    // --------------------------------------------
    // 3. PICKUP
    // --------------------------------------------
    void TryPickup()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            if (hit.rigidbody != null)
            {
                heldRb = hit.rigidbody;
                heldRb.useGravity = false;
                heldRb.linearDamping = 10f; // amortigua
            }
        }
    }

    // --------------------------------------------
    // 4. SOLTAR "LANZANDO"
    // --------------------------------------------
    void Drop()
    {
        if (heldRb != null)
        {
            heldRb.useGravity = true;
            heldRb.linearDamping = 0f;

            // Mantiene la velocidad ? se lanza
            heldRb.linearVelocity = cam.transform.forward * 6f;

            heldRb = null;
        }
    }
}
