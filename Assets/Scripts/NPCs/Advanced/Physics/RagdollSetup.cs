using UnityEngine;
using System.Collections.Generic;

namespace SmolTheftAuto.NPCs.Advanced.Physics
{
    // Manages ragdoll physics. Enables/disables ragdoll when needed (death, collision).
    public class RagdollSetup : MonoBehaviour
    {
        [Header("Ragdoll Settings")]
        [SerializeField] private List<Rigidbody> ragdollRigidbodies = new List<Rigidbody>();
        [SerializeField] private Collider mainCollider;
        [SerializeField] private float ragdollMass = 1f;

        // Cache the NavMeshAgent for disabling on death
        private UnityEngine.AI.NavMeshAgent navMeshAgent;
        private Animator animator;
        private bool isRagdollActive = false;

        private void Awake()
        {
            navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            animator = GetComponent<Animator>();
            
            // Find all rigidbodies in children for ragdoll parts
            if (ragdollRigidbodies.Count == 0)
            {
                ragdollRigidbodies.AddRange(GetComponentsInChildren<Rigidbody>());
            }

            // NEW: Initialize ragdoll as disabled (kinematic)
            DisableRagdoll();
        }

        // Disables ragdoll (kinematic mode). Used during normal movement.
        public void DisableRagdoll()
        {
            if (isRagdollActive)
            {
                // Disable all ragdoll rigidbodies
                foreach (var rb in ragdollRigidbodies)
                {
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                }

                // Re-enable main collider for normal gameplay
                if (mainCollider != null)
                    mainCollider.enabled = true;

                isRagdollActive = false;
            }
        }

        // Enables ragdoll (dynamic mode). Used on death or vehicle impact with optional force.
        public void EnableRagdoll(Vector3 forceDirection = default, float forceStrength = 0f)
        {
            if (!isRagdollActive)
            {
                // Disable main collider to prevent conflicts
                if (mainCollider != null)
                    mainCollider.enabled = false;

                // Enable all ragdoll rigidbodies
                foreach (var rb in ragdollRigidbodies)
                {
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                        rb.mass = ragdollMass;

                        // Apply force if provided (for vehicle impacts)
                        if (forceStrength > 0 && forceDirection != Vector3.zero)
                        {
                            rb.linearVelocity = forceDirection.normalized * forceStrength;
                        }
                    }
                }

                isRagdollActive = true;
            }
        }

        // Freeze NPC movement immediately (disables NavMeshAgent).
        public void FreezeMovement()
        {
            if (navMeshAgent != null && navMeshAgent.enabled)
            {
                navMeshAgent.enabled = false;
            }
        }

        // Resume NPC movement (re-enable NavMeshAgent). Called during respawn.
        public void ResumeMovement()
        {
            if (navMeshAgent != null && !navMeshAgent.enabled)
            {
                navMeshAgent.enabled = true;
                // Reset position to ensure pathfinding works correctly
                navMeshAgent.Warp(transform.position);
            }
        }

        // Check if ragdoll is currently active.
        public bool IsRagdollActive => isRagdollActive;

        private void OnDrawGizmosSelected()
        {
            // Visualize ragdoll rigidbodies in editor
            if (ragdollRigidbodies.Count > 0)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
                foreach (var rb in ragdollRigidbodies)
                {
                    if (rb != null && rb.GetComponent<SphereCollider>() != null)
                    {
                        Gizmos.DrawWireSphere(rb.transform.position, rb.GetComponent<SphereCollider>().radius);
                    }
                }
            }
        }
    }
}
