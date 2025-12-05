using UnityEngine;
using System.Collections.Generic;

namespace SmolTheftAuto.NPCs.Advanced.AI
{
    // Manages player detection for aggressive NPCs. Alerts all nearby NPCs simultaneously.
    public class PlayerDetectionZone : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private float detectionRadius = 30f;
        [SerializeField] private LayerMask playerLayer;

        [Header("Alert System")]
        [SerializeField] private float alertPropagationRadius = 50f;

        private SphereCollider detectionCollider;
        private AggressiveNPC ownerNPC;
        private List<AggressiveNPC> alertedNPCs = new List<AggressiveNPC>();
        private bool playerDetected = false;

        private void Awake()
        {
            // NEW: Setup trigger collider for player detection
            detectionCollider = GetComponent<SphereCollider>();
            if (detectionCollider == null)
            {
                detectionCollider = gameObject.AddComponent<SphereCollider>();
            }

            detectionCollider.radius = detectionRadius;
            detectionCollider.isTrigger = true;

            ownerNPC = GetComponentInParent<AggressiveNPC>();
            gameObject.layer = LayerMask.NameToLayer("Default");

            // Ensure this is a trigger
            gameObject.name = "PlayerDetectionZone";
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerDetected(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerLost();
            }
        }

        // Called when player enters detection zone. Alerts this NPC and nearby ones.
        private void OnPlayerDetected(GameObject player)
        {
            if (playerDetected) return;
            playerDetected = true;

            //Debug.Log("Player detected! Alerting aggressive NPCs...");

            // Alert this NPC
            if (ownerNPC != null)
            {
                ownerNPC.OnPlayerDetected(player);
            }

            // NEW: Alert nearby aggressive NPCs
            AlertNearbyNPCs(player);
        }

        // Called when player leaves detection zone.
        private void OnPlayerLost()
        {
            if (!playerDetected) return;
            playerDetected = false;

            //Debug.Log("Player lost!");

            if (ownerNPC != null)
            {
                ownerNPC.OnPlayerLost();
            }
        }

        // Find and alert all aggressive NPCs within alert propagation radius.
        private void AlertNearbyNPCs(GameObject player)
        {
            Collider[] nearbyColliders = UnityEngine.Physics.OverlapSphere(
                transform.position,
                alertPropagationRadius
            );

            alertedNPCs.Clear();

            foreach (Collider collider in nearbyColliders)
            {
                AggressiveNPC aggressiveNPC = collider.GetComponent<AggressiveNPC>();
                if (aggressiveNPC != null && aggressiveNPC != ownerNPC)
                {
                    aggressiveNPC.OnPlayerDetected(player);
                    alertedNPCs.Add(aggressiveNPC);
                }
            }

            //Debug.Log($"Alerted {alertedNPCs.Count} nearby aggressive NPCs");
        }

        public bool IsPlayerDetected => playerDetected;
        public float GetDetectionRadius() => detectionRadius;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            // Show alert propagation radius
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, alertPropagationRadius);
        }
    }
}
