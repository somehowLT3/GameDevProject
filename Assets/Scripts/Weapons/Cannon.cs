using UnityEngine;

public class CannonController : MonoBehaviour
{
    public Transform barrel;
    public Transform target;
    public float bodyRotateSpeed = 2f;
    public float barrelRotateSpeed = 2f;
    public float minBarrelAngle = -10f;
    public float maxBarrelAngle = 45f;
    public float maxTargetDistance = 40f;

    [Header("Firing")]
    public GameObject cannonballPrefab;
    public float fireDelay = 1.5f;
    public float projectileSpeed = 20f;
    public float explosionRadius = 3f;
    public float explosionForce = 500f;

    private Quaternion defaultBodyRotation;
    private Quaternion defaultBarrelRotation;

    private Rigidbody targetRb;
    private bool isFiring;

    void Start()
    {
        // initial rotations (to go back to when target is far away)
        defaultBodyRotation = transform.rotation;
        defaultBarrelRotation = barrel.localRotation;

        // initialise target
        targetRb = target.GetComponent<Rigidbody>();
    }

    Vector3 PredictTargetPosition()
    {
        if (targetRb == null)
            return target.position;

        Vector3 targetVelocity = targetRb.linearVelocity;

        float distance = Vector3.Distance(barrel.position, target.position);
        float timeToReach = distance / projectileSpeed;

        return target.position + targetVelocity * timeToReach;
    }

    System.Collections.IEnumerator FireRoutine()
    {
        isFiring = true;

        // wait before firing
        yield return new WaitForSeconds(fireDelay);

        if (target == null)
        {
            isFiring = false;
            yield break;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        // check range
        if (distance > maxTargetDistance)
        {
            isFiring = false;
            yield break;
        }

        // spawn slightly in front of barrel
        Vector3 spawnPos =
            barrel.position + barrel.forward * 1f;

        // spawn with barrel rotation
        GameObject ball = Instantiate(
            cannonballPrefab,
            spawnPos,
            barrel.rotation
        );

        Rigidbody rb = ball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // fire exactly where barrel points
            rb.linearVelocity =
                barrel.forward * projectileSpeed;
        }

        isFiring = false;
    }

    void Update()
    {
        if (target == null) return;

        // predict future target position
        Vector3 predictedPos = PredictTargetPosition();

        // distance check
        float distanceToTarget = Vector3.Distance(transform.position, predictedPos);

        if (distanceToTarget <= maxTargetDistance)
        {
            if (!isFiring)
                StartCoroutine(FireRoutine());


            // body rotation

            Vector3 directionToTarget = predictedPos - transform.position;

            directionToTarget.y = 0;

            if (directionToTarget.sqrMagnitude > 0.001f)
            {
                Quaternion targetBodyRotation = Quaternion.LookRotation(directionToTarget);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetBodyRotation,
                    bodyRotateSpeed * Time.deltaTime
                );
            }


            // barrel rotation

            Vector3 localTargetDir = barrel.parent.InverseTransformPoint(predictedPos);

            float targetAngle = Mathf.Atan2(localTargetDir.y, localTargetDir.z) * Mathf.Rad2Deg;

            targetAngle = Mathf.Clamp(targetAngle, minBarrelAngle, maxBarrelAngle);

            Quaternion barrelRotation = Quaternion.Euler(targetAngle, 0, 0);

            barrel.localRotation = Quaternion.Slerp(
                barrel.localRotation,
                barrelRotation,
                barrelRotateSpeed * Time.deltaTime
            );
        }
        else
        {
            // return to default rotations
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                defaultBodyRotation,
                bodyRotateSpeed * Time.deltaTime
            );

            barrel.localRotation = Quaternion.Slerp(
                barrel.localRotation,
                defaultBarrelRotation,
                barrelRotateSpeed * Time.deltaTime
            );
        }
    }
}