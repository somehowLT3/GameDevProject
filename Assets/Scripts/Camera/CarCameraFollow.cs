using UnityEngine;

public class CarCameraFollow : MonoBehaviour
{
    public Transform target;
    private Rigidbody rb;

    [Header("Position")]
    public Vector3 offset = new Vector3(0, 6, -16); // world offset
    public float followSmoothness = 5f;

    void Start()
    {
        if (target != null)
            rb = target.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        if (!target) return;

        transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

        Vector3 speed = rb != null ? rb.linearVelocity : new Vector3(0, 0, 0);
        float lookAheadZ = Mathf.Max(0, speed.z) * 0.2f;
        float lookAheadX = 0f; //speed.x * -0.2f;

        Vector3 lookAhead = new Vector3(lookAheadX, 0, lookAheadZ);

        Vector3 desiredPosition = target.position + offset + lookAhead;

        // smooth follow
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSmoothness * Time.deltaTime
        );
    }
}