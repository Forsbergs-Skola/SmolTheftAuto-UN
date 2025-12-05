using UnityEngine;
using Vehicles;
using Events;  // for IntPayloadEvent
using SmolTheftAuto.NPCs.Behavior;
namespace Vehicles
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(VehicleMover))]
    public class VehicleHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth = 100f;

        [Header("Player Death Hook")]
        [Tooltip("Hook this up to Brad's PlayerHealthChangedEvent asset.")]
        [SerializeField] private IntPayloadEvent playerHealthEvent;

        [Header("Damage From Collisions")]
        [SerializeField] private float minImpactToDamage = 3f;   
        [SerializeField] private float damageMultiplier = 5f;    
        [Header("Damage To NPCs")]
    [SerializeField] private float minImpactToDamageNPC = 2f;
    [SerializeField] private float npcDamageMultiplier = 10f;
        [Header("Death Behaviour")]
        [Tooltip("Optional override for what to destroy (if car is a parent object). If null, destroys this.gameObject.")]
        [SerializeField] private GameObject destroyRoot;

        private VehicleMover mover;
        private bool isDead = false;
        
        [SerializeField] private GameObject explosionEffect;

        // Set by PlayerVehicleController when player enters / exits
        public bool HasPlayerDriver { get; private set; }

        private void Awake()
        {
            mover = GetComponent<VehicleMover>();
            if (destroyRoot == null)
                destroyRoot = gameObject;

            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        }

        public void SetPlayerDriver(bool hasPlayer)
        {
            HasPlayerDriver = hasPlayer;
            Debug.Log($"[VehicleHealth] {name} HasPlayerDriver = {HasPlayerDriver}");
        }

    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;

        // Check if we hit an NPC/enemy by looking for NPCHealth component
        NPCHealth npcHealth = other.GetComponent<NPCHealth>();
        if (npcHealth == null)
            npcHealth = other.GetComponentInParent<NPCHealth>();

        if (npcHealth != null && !npcHealth.IsDestroyed())
        {
            // Car damages NPC
            float impact = collision.relativeVelocity.magnitude;

            if (impact >= minImpactToDamageNPC)
            {
                float damageToNPC = (impact - minImpactToDamageNPC) * npcDamageMultiplier;
                Debug.Log($"[VehicleHealth] {name} hit NPC {npcHealth.gameObject.name} for {damageToNPC} damage. Impact: {impact}");
                npcHealth.Health -= damageToNPC;
            }
            return;
        }

        
        float selfImpact = collision.relativeVelocity.magnitude;

    if (selfImpact < minImpactToDamage)
        return;

    float damageToSelf = (selfImpact - minImpactToDamage) * damageMultiplier;
    ApplyDamage(damageToSelf);
}

        private void OnTriggerEnter(Collider collider)
{
    
    NPCHealth npcHealth = collider.GetComponent<NPCHealth>();
    if (npcHealth == null)
        npcHealth = collider.GetComponentInParent<NPCHealth>();

    if (npcHealth != null && !npcHealth.IsDestroyed())
    {
        
        float impact = GetComponent<Rigidbody>().linearVelocity.magnitude;
        
        if (impact >= minImpactToDamageNPC)
        {
            float damageToNPC = (impact - minImpactToDamageNPC) * npcDamageMultiplier;
            Debug.Log($"[VehicleHealth] {name} hit trigger NPC {npcHealth.gameObject.name} for {damageToNPC} damage. Impact: {impact}");
            npcHealth.Health -= damageToNPC;
        }
    }
}

        public void ApplyDamage(float damage)
        {
            if (damage <= 0f || isDead) return;

            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0f);

            Debug.Log($"[VehicleHealth] {name} took {damage} damage, HP = {currentHealth}");

            if (currentHealth <= 0f)
            {
                HandleDeath();
            }
        }

        private void HandleDeath()
        {
            if (isDead) return;
            isDead = true;

            Debug.Log(
                $"[VehicleHealth] HandleDeath on {name}. " +
                $"HasPlayerDriver={HasPlayerDriver}, " +
                $"playerHealthEvent={(playerHealthEvent == null ? "NULL" : playerHealthEvent.name)}"
            );

            // If the player is currently inside this vehicle trigger Brads health event
            if (HasPlayerDriver && playerHealthEvent != null)
            {
                Debug.Log("[VehicleHealth] Triggering playerHealthEvent(-1000)");
                playerHealthEvent.TriggerEvent(-1000);
            }
            
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            AudioManager.Instance.GunshotSFX("CarBoom", transform.position);

            // Despawn / destroy the car (NPC or player car)
            if (destroyRoot != null)
                Destroy(destroyRoot);
            else
                Destroy(gameObject);
                
                
        }

        public void Heal(float amount)
        {
            if (amount <= 0f || isDead) return;
            currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        }
    }
}
