using UnityEngine;

public class Cannonball : MonoBehaviour
{
    public float explosionRadius = 10f;
    public float explosionForce = 500f;
    public GameObject explosionEffect;
    public GameObject explosionDamagePrefab;

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius/4);

        foreach (Collider hit in hits)
        {
            bool doesHit = hit.TryGetComponent<Rigidbody>(out var rb);
            if (doesHit)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        if (explosionDamagePrefab != null)
        {
            GameObject damageArea = Instantiate(
                explosionDamagePrefab,
                transform.position,
                Quaternion.identity
            );

            damageArea.transform.localScale = Vector3.one * explosionRadius;
        }

        Destroy(gameObject);
    }
}