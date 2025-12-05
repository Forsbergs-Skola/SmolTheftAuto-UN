using UnityEngine;
using SmolTheftAuto.NPCs.Behavior;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float explosionDelay = 3f;
    [SerializeField] private float countdown;
    [SerializeField] private float explosionRadius = 50f;
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private float damage = 10f;

    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private GameObject confettiEffect;

    [SerializeField] private bool hasExploded; 
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
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Instantiate(confettiEffect, transform.position, Quaternion.identity);
        AudioManager.Instance.GunshotSFX("GrenadeBoom", transform.position);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            
            if (hit.gameObject.TryGetComponent(out NPCHealth enemy))
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
