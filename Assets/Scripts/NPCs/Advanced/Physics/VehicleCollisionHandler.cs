using UnityEngine;

namespace SmolTheftAuto.NPCs.Advanced.Physics
{
    // Handles vehicle collisions. Applies ragdoll physics when vehicle hits NPC.
    public class VehicleCollisionHandler : MonoBehaviour
    {
        [Header("Collision Settings")]
        [SerializeField] private float baseForceStrength = 20f;
        [SerializeField] private float forceScaleWithVelocity = 1.5f;
        [SerializeField] private LayerMask vehicleLayer = -1;

        [Header("References")]
        private RagdollSetup ragdollSetup;
        private UnityEngine.AI.NavMeshAgent navMeshAgent;

        private void Awake()
        {
            ragdollSetup = GetComponent<RagdollSetup>();
            navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

            if (ragdollSetup == null)
            {
                Debug.LogWarning($"{nameof(VehicleCollisionHandler)} requires {nameof(RagdollSetup)} on same GameObject.", this);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Check if colliding object is a vehicle
            if (((1 << collision.gameObject.layer) & vehicleLayer) != 0)
            {
                HandleVehicleCollision(collision);
            }
        }

        // Handle physics response when vehicle hits NPC. Calculates impact force and applies ragdoll.
        private void HandleVehicleCollision(Collision collision)
        {
            // NEW: Get the vehicle's rigidbody for velocity
            Rigidbody vehicleRb = collision.rigidbody;
            if (vehicleRb == null)
            {
                Debug.LogWarning("Vehicle collision detected but vehicle has no Rigidbody.", this);
                return;
            }

            // Calculate impact force from vehicle velocity
            Vector3 vehicleVelocity = vehicleRb.linearVelocity;
            float impactForce = baseForceStrength + (vehicleVelocity.magnitude * forceScaleWithVelocity);

            // Determine throw direction (away from vehicle)
            Vector3 throwDirection = (transform.position - collision.transform.position).normalized;
            if (throwDirection == Vector3.zero)
                throwDirection = Vector3.forward;

            // NEW: Freeze movement immediately
            if (ragdollSetup != null)
            {
                ragdollSetup.FreezeMovement();
            }

            // NEW: Enable ragdoll with force
            if (ragdollSetup != null)
            {
                ragdollSetup.EnableRagdoll(throwDirection, impactForce);
            }

            Debug.Log($"NPC hit by vehicle. Impact force: {impactForce}, Direction: {throwDirection}");
        }

        // NEW: Public method to apply custom force (for other collision types)
        public void ApplyCustomForce(Vector3 forceDirection, float forceStrength)
        {
            if (ragdollSetup != null)
            {
                ragdollSetup.FreezeMovement();
                ragdollSetup.EnableRagdoll(forceDirection, forceStrength);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 2f);
        }
    }
}
