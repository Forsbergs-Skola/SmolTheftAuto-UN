using UnityEngine;
using System.Collections.Generic;

namespace SmolTheftAuto.NPCs.Advanced.Physics
{
    /// <summary>
    /// Manages the ragdoll system for NPCs. Enables/disables ragdoll physics
    /// when appropriate (death, vehicle collision, etc).
    /// </summary>
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

        /// <summary>
        /// Disables ragdoll physics (kinematic mode). Used during normal movement.
        /// </summary>
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

        /// <summary>
        /// Enables ragdoll physics (dynamic mode). Used on death or vehicle impact.
        /// Applies optional force for vehicle collisions.
        /// </summary>
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

        /// <summary>
        /// Instantly freeze NPC movement (disables NavMeshAgent).
        /// Called immediately upon death before animations.
        /// </summary>
        public void FreezeMovement()
        {
            if (navMeshAgent != null && navMeshAgent.enabled)
            {
                navMeshAgent.enabled = false;
            }
        }

        /// <summary>
        /// Resumes NPC movement (re-enables NavMeshAgent).
        /// Called during respawn.
        /// </summary>
        public void ResumeMovement()
        {
            if (navMeshAgent != null && !navMeshAgent.enabled)
            {
                navMeshAgent.enabled = true;
                // Reset position to ensure pathfinding works correctly
                navMeshAgent.Warp(transform.position);
            }
        }

        /// <summary>
        /// Check if ragdoll is currently active.
        /// </summary>
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
