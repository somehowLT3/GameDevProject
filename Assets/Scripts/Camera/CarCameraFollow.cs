using UnityEngine;

public class CarCameraFollow : MonoBehaviour
{
    public Transform target;
    public Transform target2;
    private Rigidbody rb;

    [Header("Position")]
    public Vector3 offset = new Vector3(0, 6, -16); // world offset
    public float followSmoothness = 5f;

    private Transform t;

    void Start()
    {
        if (target != null)
            rb = target.GetComponent<Rigidbody>();

        if (GameSettings.useSportsCar)
        {
            t = target2;
            return;
        }
        t = target;
    }

    void LateUpdate()
    {
        if (!t) return;

        transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

        Vector3 speed = rb != null ? rb.linearVelocity : new Vector3(0, 0, 0);
        float lookAheadZ = Mathf.Max(0, speed.z) * 0.2f;
        float lookAheadX = 0f; //speed.x * -0.2f;

        Vector3 lookAhead = new Vector3(lookAheadX, 0, lookAheadZ);

        Vector3 desiredPosition = t.position + offset + lookAhead;

        // smooth follow
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSmoothness * Time.deltaTime
        );
    }
}