using UnityEngine;
using Vehicles;
using Events;  // for EmptyPayloadEvent

namespace Vehicles
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(VehicleMover))]
    public class VehicleHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth = 100f;

        [Header("Damage From Collisions")]
        [SerializeField] private float minImpactToDamage = 3f;   // ignore tiny bumps
        [SerializeField] private float damageMultiplier = 5f;    // tweak this

        [Header("Death Behaviour")]
        [Tooltip("If true and this vehicle has the player driving, we call playerDeathEvent instead of just destroying the car.")]
        [SerializeField] private EmptyPayloadEvent playerDeathEvent;
        [Tooltip("Optional override for what to destroy (if car is a parent object). If null, destroys this.gameObject.")]
        [SerializeField] private GameObject destroyRoot;

        private VehicleMover mover;
        
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
            if (damage <= 0f) return;

            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0f);

             Debug.Log($"{name} took {damage} damage, HP = {currentHealth}");

            if (currentHealth <= 0f)
            {
                HandleDeath();
            }
        }

        private void HandleDeath()
        {
            // If the player is currently inside this vehicle  player dies
            if (HasPlayerDriver && playerDeathEvent != null)
            {
                // player Died event goes here
            }

            // Despawn / destroy the car (works for NPC or empty cars)
            if (destroyRoot != null)
            {
                Destroy(destroyRoot);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Heal(float amount)
        {
            if (amount <= 0f) return;
            currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        }
    }
}
