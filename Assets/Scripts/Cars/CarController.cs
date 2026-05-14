using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Speed")]
    public float acceleration = 25f;
    public float maxSpeed = 20f;
    public float reverseSpeed = 10f;

    [Header("Steering")]
    public float turnSpeed = 180f;
    public float driftFactor = 0.75f; // essent. coeeff of friction
    public float driftControl = 2f;  // how fast sideways slip is corrected (seems to act weird?)

    public List<Material> healthMaterials;

    [Header("Death")]
    public GameObject wreckPrefab;
    public float explosionForce = 500f;
    public float explosionRadius = 5f;

    [Header("Damage")]
    public float damageCooldown = 0.5f;

    private float lastHitTime = -999f;

    private Renderer carRenderer;

    private Rigidbody rb;
    private CarControls controls;
    private Vector2 moveInput;
    private int health;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new CarControls();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        controls.Driving.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Driving.Move.canceled += ctx => moveInput = Vector2.zero;

        rb.linearDamping = 0;
        rb.angularDamping = 0;

        // max lives (safe material check)
        health = Mathf.Min(GameSettings.maxLives, healthMaterials.Count - 1);

        carRenderer = GetComponentInChildren<Renderer>();

        carRenderer = GetComponentInChildren<Renderer>();

        ChangeCarMaterial();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleSteering();
        ApplyDrift();
    }

    void HandleMovement()
    {
        float forwardInput = moveInput.y;

        Vector3 velocity = rb.linearVelocity;

        // modify forward component only
        float currentForwardSpeed = Vector3.Dot(velocity, transform.forward);

        float newForwardSpeed = Mathf.Clamp(
            currentForwardSpeed + forwardInput * acceleration * Time.fixedDeltaTime,
            -reverseSpeed,
            maxSpeed
        );

        Vector3 newVelocity = transform.forward * newForwardSpeed + GetSidewaysVelocity();

        rb.linearVelocity = newVelocity;
    }

    void HandleSteering()
    {
        float turnInput = moveInput.x;

        // increase stearing with speed
        float speedPercent = rb.linearVelocity.magnitude / maxSpeed;
        float speedAdjustedTurn = turnSpeed * speedPercent;

        transform.Rotate(Vector3.up, turnInput * speedAdjustedTurn * Time.fixedDeltaTime);
    }

    void ApplyDrift()
    {
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(rb.linearVelocity, transform.forward);
        Vector3 sidewaysVelocity = transform.right * Vector3.Dot(rb.linearVelocity, transform.right);

        // lower sideways slipping
        sidewaysVelocity *= driftFactor;

        rb.linearVelocity = forwardVelocity + sidewaysVelocity;
    }

    void ChangeCarMaterial()
    {
        carRenderer.material = healthMaterials[health -1];
    }

    void ExplodeCar()
    {
        GameObject wreck = Instantiate(
            wreckPrefab,
            transform.position,
            transform.rotation
        );

        Rigidbody[] parts =
            wreck.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in parts)
        {
            rb.AddExplosionForce(
                explosionForce,
                transform.position,
                explosionRadius
            );

            rb.AddTorque(
                Random.insideUnitSphere * explosionForce
            );
        }

        Destroy(gameObject);
    }

    void OnHit()
    {
        // stop instant damage twice
        if (Time.time < lastHitTime + damageCooldown)
            return;

        lastHitTime = Time.time;

        if (health > 1)
        {
            health -= 1;
            ChangeCarMaterial();
            return;
        }

        // death effect
        ExplodeCar();

        FindFirstObjectByType<UIManager>().ShowFailScreen();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            OnHit();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damage"))
        {
            OnHit();
            return;
        }

        if (other.CompareTag("Finish"))
        {
            FindFirstObjectByType<UIManager>().ShowSuccessScreen();
            controls.Disable();
        }
    }

    Vector3 GetSidewaysVelocity()
    {
        return transform.right * Vector3.Dot(rb.linearVelocity, transform.right);
    }
}