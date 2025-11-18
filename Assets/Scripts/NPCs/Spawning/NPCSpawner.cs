using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmolTheftAuto.Core;
using Events;

namespace SmolTheftAuto.NPCs.Spawning
{
    // Handles spawning and respawning of NPCs at random locations
    public class NPCSpawner : MonoBehaviour, INPCSpawner
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject npcPrefab;
        [SerializeField] private int initialNPCCount = 10;
        [SerializeField] private int maxNPCCount = 20;
        [SerializeField] private float spawnRadius = 50f;
        [SerializeField] private float minSpawnDistanceFromPlayer = 10f;
        [SerializeField] private LayerMask spawnCheckLayers = -1;

        [Header("Spawn Area")]
        [SerializeField] private Vector3 spawnCenter = Vector3.zero;
        [SerializeField] private bool useSpawnCenter = false;

        [Header("Event Channels")]
        [SerializeField] private GameObjectPayloadEvent npcDestroyedEvent;

        private List<GameObject> activeNPCs = new List<GameObject>();

        private void Awake()
        {
            ServiceLocator.Register<INPCSpawner>(this);
        }

        private void OnEnable()
        {
            if (npcDestroyedEvent != null)
            {
                npcDestroyedEvent.OnEventTriggered += OnNPCDestroyed;
            }
        }

        private void OnDisable()
        {
            if (npcDestroyedEvent != null)
            {
                npcDestroyedEvent.OnEventTriggered -= OnNPCDestroyed;
            }
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<INPCSpawner>();
        }

        private void Start()
        {
            for (int i = 0; i < initialNPCCount; i++)
            {
                SpawnNPC();
            }
        }

        // Spawn a new NPC at a random location
        public GameObject SpawnNPC()
        {
            if (npcPrefab == null)
            {
                Debug.LogWarning("NPC Prefab is not assigned in NPCSpawner!");
                return null;
            }

            if (activeNPCs.Count >= maxNPCCount)
            {
                return null;
            }

            if (!TryGetRandomSpawnPosition(out Vector3 spawnPosition))
            {
                Debug.LogWarning("Could not find valid spawn position for NPC!");
                return null;
            }

            GameObject npc = Instantiate(npcPrefab, spawnPosition, Quaternion.identity);
            activeNPCs.Add(npc);

            return npc;
        }

        // Respawn an NPC after a delay
        public void RespawnNPC(GameObject npc, float delay)
        {
            StartCoroutine(RespawnNPCCoroutine(npc, delay));
        }

        private IEnumerator RespawnNPCCoroutine(GameObject npc, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (activeNPCs.Contains(npc))
            {
                activeNPCs.Remove(npc);
            }

            if (npc != null)
            {
                Destroy(npc);
            }

            SpawnNPC();
        }

        // Get a random valid spawn position
        private bool TryGetRandomSpawnPosition(out Vector3 spawnPosition)
        {
            Vector3 center;
            if (useSpawnCenter)
            {
                center = spawnCenter;
            }
            else if (PlayerReference.PlayerTransform != null)
            {
                center = PlayerReference.PlayerTransform.position;
            }
            else
            {
                center = transform.position;
            }

            for (int attempts = 0; attempts < 30; attempts++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
                Vector3 candidatePosition = center + new Vector3(randomCircle.x, 0, randomCircle.y);

                if (IsValidSpawnPosition(candidatePosition) &&
                    Physics.Raycast(candidatePosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f, spawnCheckLayers))
                {
                    spawnPosition = hit.point;
                    return true;
                }
            }

            spawnPosition = Vector3.zero;
            return false;
        }

        // Check if a position is valid for spawning
        private bool IsValidSpawnPosition(Vector3 position)
        {
            if (PlayerReference.PlayerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(position, PlayerReference.PlayerTransform.position);
                if (distanceToPlayer < minSpawnDistanceFromPlayer)
                {
                    return false;
                }
            }

            return true;
        }

        // Handle when an NPC is destroyed (called from event)
        private void OnNPCDestroyed(GameObject npc)
        {
            if (activeNPCs.Contains(npc))
            {
                activeNPCs.Remove(npc);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Vector3 center = useSpawnCenter ? spawnCenter : transform.position;
            Gizmos.DrawWireSphere(center, spawnRadius);
            
            Gizmos.color = Color.yellow;
            if (PlayerReference.PlayerTransform != null)
            {
                Gizmos.DrawWireSphere(PlayerReference.PlayerTransform.position, minSpawnDistanceFromPlayer);
            }
        }
    }
}


