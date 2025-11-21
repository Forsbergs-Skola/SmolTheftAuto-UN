using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float explosionDelay = 3f;
    private float countdown;
    public float explosionRadius = 50f;
    public float explosionForce = 700f;
    public float damage = 10f;

    public GameObject explosionEffect;

    private bool hasExploded; 
    void Start() => countdown = explosionDelay;
    
    void Update()
    {
        countdown -= Time.deltaTime;

        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            
            if (hit.gameObject.TryGetComponent(out Enemy enemy))
                enemy.Health -= damage;
        }
        Destroy(gameObject);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
    
}
