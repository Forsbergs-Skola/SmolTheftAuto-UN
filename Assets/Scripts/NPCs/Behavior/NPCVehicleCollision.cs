using UnityEngine;
using SmolTheftAuto.Core;

namespace SmolTheftAuto.NPCs.Behavior
{
    // Component to handle vehicle collisions with NPCs
    // NPCs can be damaged and destroyed by player vehicles
    // NPCs do NOT damage vehicles
    [RequireComponent(typeof(NPCHealth))]
    public class NPCVehicleCollision : MonoBehaviour
    {
        [Header("Vehicle Collision Settings")]
        [SerializeField] private float vehicleDamageMultiplier = 100f;
        [SerializeField] private float minImpactVelocity = 5f;
        [SerializeField] private LayerMask vehicleLayer = -1;

        private NPCHealth npcHealth;
        private Rigidbody npcRigidbody;

        private void Awake()
        {
            npcHealth = GetComponent<NPCHealth>();
            npcRigidbody = GetComponent<Rigidbody>();
        }

        // Handle collision with vehicles
        private void OnCollisionEnter(Collision collision)
        {
            // Check if collision is with a vehicle
            if (!IsVehicle(collision.gameObject))
            {
                return;
            }

            // Calculate impact force
            float impactForce = CalculateImpactForce(collision);

            if (impactForce >= minImpactVelocity)
            {
                // Calculate damage based on impact force
                float damage = impactForce * vehicleDamageMultiplier;

                // Deal damage to NPC
                if (npcHealth != null)
                {
                    npcHealth.TakeDamage(damage);
                }

                // Apply force to NPC (optional - for ragdoll effect)
                if (npcRigidbody != null)
                {
                    Vector3 impactDirection = collision.contacts[0].normal;
                    npcRigidbody.AddForce(-impactDirection * impactForce, ForceMode.Impulse);
                }
            }
        }

        // Check if the colliding object is a vehicle
        private bool IsVehicle(GameObject obj)
        {
            // Check by layer
            if (vehicleLayer == (vehicleLayer | (1 << obj.layer)))
            {
                return true;
            }

            // Check by tag (if vehicles have a specific tag)
            if (obj.CompareTag("Vehicle"))
            {
                return true;
            }

            // Check by component (if vehicles have a specific component)
            // You can add more vehicle detection methods here

            return false;
        }

        // Calculate the impact force from collision
        private float CalculateImpactForce(Collision collision)
        {
            float impactForce = 0f;

            // Try to get velocity from the vehicle's rigidbody
            Rigidbody vehicleRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (vehicleRigidbody != null)
            {
                // Calculate relative velocity
                Vector3 relativeVelocity = collision.relativeVelocity;
                impactForce = relativeVelocity.magnitude;
            }
            else if (npcRigidbody != null)
            {
                // Fallback: use collision relative velocity
                impactForce = collision.relativeVelocity.magnitude;
            }

            return impactForce;
        }
    }
}

