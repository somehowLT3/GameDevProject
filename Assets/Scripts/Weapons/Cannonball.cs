using UnityEngine;

public class Cannonball : MonoBehaviour
{
    public float explosionRadius = 3f;
    public float explosionForce = 500f;
    public GameObject explosionEffect;

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        //Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius/4);

        //foreach (Collider hit in hits)
        //{
        //    Rigidbody rb = hit.GetComponent<Rigidbody>();

        //    if (rb != null)
        //    {
        //        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        //    }
        //}

        //if (explosionEffect != null)
        //    Instantiate(explosionEffect, transform.position, Quaternion.identity);

        //Destroy(gameObject);
    }
}