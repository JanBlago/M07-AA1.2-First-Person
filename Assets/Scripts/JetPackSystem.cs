using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class JetpackSystem : MonoBehaviour
{
    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;

    [Header("Jump Settings")]
    public float jumpForce = 6f;

    [Header("Jetpack Settings")]
    public float jetpackForce = 10f;
    public float fuelMax = 1f;                 // 1 = 100%
    public float fuelBurnTime = 1f;            // tarda 1 segundo en gastarse
    public float fuelRechargeTime = 0.5f;      // tarda 0.5s en recargarse
    public float rechargeDelay = 0.5f;         // delay si llega a 0

    [Header("UI")]
    public Image fuelUI;                       // radial UI

    private float fuel;
    private bool isRecharging = false;
    private float lastGroundTime;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        fuel = fuelMax;
    }

    private void Update()
    {
        bool isGrounded = CheckGrounded();

        HandleJump(isGrounded);
        HandleJetpack(isGrounded);
        UpdateUI();
    }

    // ------------------------------------------------------
    // GROUND CHECK
    // ------------------------------------------------------
    bool CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundLayer);
    }

    // ------------------------------------------------------
    // NORMAL JUMP
    // ------------------------------------------------------
    void HandleJump(bool grounded)
    {
        if (grounded && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    // ------------------------------------------------------
    // JETPACK
    // ------------------------------------------------------
    void HandleJetpack(bool grounded)
    {
        if (grounded)
        {
            HandleFuelRecharge();
            return;
        }

        // Activar jetpack solo si tienes combustible
        if (Keyboard.current.spaceKey.isPressed && fuel > 0)
        {
            UseJetpack();
        }
    }

    void UseJetpack()
    {
        // Fuerza hacia arriba
        rb.AddForce(Vector3.up * jetpackForce, ForceMode.Acceleration);

        // Consumir combustible
        fuel -= Time.deltaTime / fuelBurnTime;
        fuel = Mathf.Clamp(fuel, 0, fuelMax);

        // Si se queda sin combustible → delay antes de recargar
        if (fuel <= 0 && !isRecharging)
        {
            isRecharging = true;
            lastGroundTime = Time.time;
        }
    }

    // ------------------------------------------------------
    // RECARGA DE COMBUSTIBLE AL TOCAR SUELO
    // ------------------------------------------------------
    void HandleFuelRecharge()
    {
        // Si el combustible NO está al 0, puede recargar inmediatamente
        if (!isRecharging)
        {
            if (fuel < fuelMax)
                fuel += Time.deltaTime / fuelRechargeTime;

            fuel = Mathf.Clamp(fuel, 0, fuelMax);
            return;
        }

        // Si estaba al 0 → tiene que esperar rechargeDelay
        if (Time.time - lastGroundTime >= rechargeDelay)
        {
            fuel += Time.deltaTime / fuelRechargeTime;

            if (fuel >= fuelMax)
            {
                fuel = fuelMax;
                isRecharging = false;
            }
        }
    }

    // ------------------------------------------------------
    // UI RADIAL
    // ------------------------------------------------------
    void UpdateUI()
    {
        if (fuelUI != null)
            fuelUI.fillAmount = fuel / fuelMax;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + Vector3.down * groundCheckDistance);
    }
}
