using System.Collections.Generic;
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

    private Quaternion defaultBodyRotation;
    private Quaternion defaultBarrelRotation;

    void Start()
    {
        // initial rotations (to go back to when target is far away)
        defaultBodyRotation = transform.rotation;
        defaultBarrelRotation = barrel.localRotation;
    }

    void Update()
    {
        if (target == null) return;

        // get distance to target
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= maxTargetDistance)
        {
            // rotate body (y only)
            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.y = 0; // only horizontally
            if (directionToTarget.sqrMagnitude > 0.001f)
            {
                Quaternion targetBodyRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetBodyRotation, bodyRotateSpeed * Time.deltaTime);
            }

            // barrel (x only)
            Vector3 localTargetDir = barrel.parent.InverseTransformPoint(target.position);
            float targetAngle = Mathf.Atan2(localTargetDir.y, localTargetDir.z) * Mathf.Rad2Deg;
            targetAngle = Mathf.Clamp(targetAngle, minBarrelAngle, maxBarrelAngle);

            Quaternion barrelRotation = Quaternion.Euler(targetAngle, 0, 0);
            barrel.localRotation = Quaternion.Slerp(barrel.localRotation, barrelRotation, barrelRotateSpeed * Time.deltaTime);
        }
        else
        {
            // return to default rotations
            transform.rotation = Quaternion.Slerp(transform.rotation, defaultBodyRotation, bodyRotateSpeed * Time.deltaTime);
            barrel.localRotation = Quaternion.Slerp(barrel.localRotation, defaultBarrelRotation, barrelRotateSpeed * Time.deltaTime);
        }
    }
}