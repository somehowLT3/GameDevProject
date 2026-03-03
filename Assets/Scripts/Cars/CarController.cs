using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Stats")]
    public int health = 2;

    [Header("Visuals")]
    public Material materialHealth1;
    public Material materialHealth2;

    // todo: public list<Material>, private health = size()

    private Renderer carRenderer;

    private Rigidbody rb;
    private CarControls controls;
    private Vector2 moveInput;

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

        Vector3 forwardVelocity = transform.forward * forwardInput * acceleration;

        Vector3 velocity = rb.linearVelocity;

        // modify forward component only
        float currentForwardSpeed = Vector3.Dot(velocity, transform.forward);

        float targetSpeed = forwardInput > 0 ? maxSpeed : reverseSpeed;

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
        switch (health)
        {
            case 2:
                carRenderer.material = materialHealth2;
                break;
            case 1:
                carRenderer.material = materialHealth1;
                break;
            default:
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            health = health - 1;
            ChangeCarMaterial();
        }
    }

    Vector3 GetSidewaysVelocity()
    {
        return transform.right * Vector3.Dot(rb.linearVelocity, transform.right);
    }
}