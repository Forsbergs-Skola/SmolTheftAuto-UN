using UnityEngine;
using UnityEngine.AI;
using SmolTheftAuto.NPCs.Data;

namespace SmolTheftAuto.NPCs.Behavior
{
    // Component responsible for NPC movement behavior (patrol/wander)
    // Uses NavMesh for pathfinding and wandering
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float movementSpeed = 3f;
        [SerializeField] private float wanderRadius = 10f;
        [SerializeField] private float wanderTimer = 5f;
        [SerializeField] private bool useNavMesh = true;

        private NavMeshAgent navAgent;
        private NPCData npcData;
        private NPCHealth npcHealth;
        private float timer;
        private Vector3 startPosition;

        private void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
            npcHealth = GetComponent<NPCHealth>();
            startPosition = transform.position;

            // Try to get NPCData from parent NPCController
            var npcController = GetComponent<NPCController>();
            if (npcController != null)
            {
                npcData = npcController.GetNPCData();
            }
        }

        private void Start()
        {
            // Apply NPCData settings if available
            if (npcData != null)
            {
                movementSpeed = npcData.movementSpeed;
                wanderRadius = npcData.wanderRadius;
                wanderTimer = npcData.wanderTimer;
            }

            if (useNavMesh && navAgent != null)
            {
                navAgent.enabled = true;
                navAgent.speed = movementSpeed;
            }
        }

        private void Update()
        {
            // Don't move if destroyed
            if (npcHealth != null && npcHealth.IsDestroyed())
            {
                if (navAgent != null && navAgent.isActiveAndEnabled)
                {
                    navAgent.enabled = false;
                }
                return;
            }

            if (!useNavMesh || navAgent == null || !navAgent.enabled) return;

            timer += Time.deltaTime;

            // Set new destination when timer expires
            if (timer >= wanderTimer)
            {
                Vector3 newPos = GetRandomPosition();
                if (navAgent.isOnNavMesh)
                {
                    navAgent.SetDestination(newPos);
                }
                timer = 0;
            }
        }

        // Get a random position within wander radius
        private Vector3 GetRandomPosition()
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += startPosition;
            randomDirection.y = startPosition.y; // Keep Y at ground level
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
            {
                return hit.position;
            }
            
            return startPosition;
        }

        // Set the starting position for wandering
        public void SetStartPosition(Vector3 position)
        {
            startPosition = position;
        }

        // Stop movement (useful when NPC is destroyed)
        public void StopMovement()
        {
            if (navAgent != null && navAgent.isActiveAndEnabled)
            {
                navAgent.enabled = false;
            }
        }
    }
}

