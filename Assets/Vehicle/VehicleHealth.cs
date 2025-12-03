using UnityEngine;
using Vehicles;
using Events;  // for IntPayloadEvent

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
        [SerializeField] private float minImpactToDamage = 3f;   // ignore tiny bumps
        [SerializeField] private float damageMultiplier = 5f;    // tweak this

        [Header("Death Behaviour")]
        [Tooltip("Optional override for what to destroy (if car is a parent object). If null, destroys this.gameObject.")]
        [SerializeField] private GameObject destroyRoot;

        private VehicleMover mover;
        private bool isDead = false;

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
            // Basic impact damage using relative velocity magnitude
            float impact = collision.relativeVelocity.magnitude;

            if (impact < minImpactToDamage)
                return;

            float damage = (impact - minImpactToDamage) * damageMultiplier;
            ApplyDamage(damage);
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

            // If the player is currently inside this vehicle, trigger Brad's health event
            if (HasPlayerDriver && playerHealthEvent != null)
            {
                Debug.Log("[VehicleHealth] Triggering playerHealthEvent(-1000)");
                playerHealthEvent.TriggerEvent(-1000);
            }

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
