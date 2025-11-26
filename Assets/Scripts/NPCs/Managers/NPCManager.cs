using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmolTheftAuto.Core;
using Events;
using System;

namespace SmolTheftAuto.NPCs.Managers
{
    // Centralized manager for NPC spawning, tracking, and respawning
    // Uses ServiceLocator pattern for decoupled access
    // Supports save/load functionality for VG requirements
    public class NPCManager : MonoBehaviour, INPCSpawner, ISaveable
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
        private Dictionary<GameObject, Behavior.NPCHealth> npcHealthMap = new Dictionary<GameObject, Behavior.NPCHealth>();
        private Queue<RespawnData> npcsPendingRespawn = new Queue<RespawnData>();
        private Coroutine respawnProcessor = null;

        public int ActiveNPCCount => activeNPCs.Count;
        public int MaxNPCCount => maxNPCCount;

        // Data structure for respawn queue
        private struct RespawnData
        {
            public float respawnTime;
            public float delay;
        }

        private void Awake()
        {
            ServiceLocator.Register<INPCSpawner>(this);
        }

        private void OnDestroy()
        {
            // Clean up event subscriptions
            UnsubscribeAllNPCs();
            
            // Stop respawn processor
            if (respawnProcessor != null)
            {
                StopCoroutine(respawnProcessor);
                respawnProcessor = null;
            }
            
            ServiceLocator.Unregister<INPCSpawner>();
        }

        private void Start()
        {
            // Spawn initial NPCs
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
                Debug.LogWarning("NPC Prefab is not assigned in NPCManager!", this);
                return null;
            }

            if (activeNPCs.Count >= maxNPCCount)
            {
                return null;
            }

            if (!TryGetRandomSpawnPosition(out Vector3 spawnPosition))
            {
                Debug.LogWarning("Could not find valid spawn position for NPC!", this);
                return null;
            }

            GameObject npc = Instantiate(npcPrefab, spawnPosition, Quaternion.identity);
            activeNPCs.Add(npc);

            // Register for NPC destroyed events if available
            var npcHealth = npc.GetComponent<Behavior.NPCHealth>();
            if (npcHealth != null)
            {
                npcHealth.OnNPCDestroyed += HandleNPCDestroyed;
                npcHealthMap[npc] = npcHealth;
            }

            return npc;
        }

        // Respawn an NPC after a delay
        public void RespawnNPC(GameObject npc, float delay)
        {
            if (npc == null || delay < 0) return;

            // Unsubscribe from events before removing
            UnsubscribeFromNPC(npc);

            // Remove from active list if it's there
            if (activeNPCs.Contains(npc))
            {
                activeNPCs.Remove(npc);
            }

            // Remove from health map
            if (npcHealthMap.ContainsKey(npc))
            {
                npcHealthMap.Remove(npc);
            }

            // Queue for respawn with timestamp
            RespawnData respawnData = new RespawnData
            {
                respawnTime = Time.time + delay,
                delay = delay
            };
            npcsPendingRespawn.Enqueue(respawnData);

            // Start respawn processor if not already running
            if (respawnProcessor == null)
            {
                respawnProcessor = StartCoroutine(RespawnProcessorCoroutine());
            }
        }

        // Single coroutine that processes all respawns
        private IEnumerator RespawnProcessorCoroutine()
        {
            while (npcsPendingRespawn.Count > 0)
            {
                // Create a list to hold respawns we'll process this frame
                List<RespawnData> readyRespawns = new List<RespawnData>();
                Queue<RespawnData> remainingRespawns = new Queue<RespawnData>();

                // Separate ready respawns from those still waiting
                while (npcsPendingRespawn.Count > 0)
                {
                    RespawnData respawn = npcsPendingRespawn.Dequeue();
                    if (Time.time >= respawn.respawnTime)
                    {
                        readyRespawns.Add(respawn);
                    }
                    else
                    {
                        remainingRespawns.Enqueue(respawn);
                    }
                }

                // Put remaining respawns back in the queue
                while (remainingRespawns.Count > 0)
                {
                    npcsPendingRespawn.Enqueue(remainingRespawns.Dequeue());
                }

                // Process all ready respawns
                foreach (var respawn in readyRespawns)
                {
                    SpawnNPC();
                }

                // Wait a bit before checking again (or wait for next respawn if queue not empty)
                if (npcsPendingRespawn.Count > 0)
                {
                    RespawnData nextRespawn = npcsPendingRespawn.Peek();
                    float waitTime = Mathf.Max(0.1f, nextRespawn.respawnTime - Time.time);
                    yield return new WaitForSeconds(waitTime);
                }
                else
                {
                    yield return null;
                }
            }

            respawnProcessor = null;
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
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * spawnRadius;
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

        // Handle when an NPC is destroyed
        private void HandleNPCDestroyed(GameObject npc)
        {
            if (npc == null) return;

            // Unsubscribe from events
            UnsubscribeFromNPC(npc);

            // Remove from active list
            if (activeNPCs.Contains(npc))
            {
                activeNPCs.Remove(npc);
            }

            // Remove from health map
            if (npcHealthMap.ContainsKey(npc))
            {
                npcHealthMap.Remove(npc);
            }

            // Trigger event for quest system
            npcDestroyedEvent?.TriggerEvent(npc);
        }

        // Remove NPC from tracking (called when NPC is destroyed externally)
        public void RemoveNPC(GameObject npc)
        {
            if (npc == null) return;

            UnsubscribeFromNPC(npc);

            if (activeNPCs.Contains(npc))
            {
                activeNPCs.Remove(npc);
            }

            if (npcHealthMap.ContainsKey(npc))
            {
                npcHealthMap.Remove(npc);
            }
        }

        // Unsubscribe from a specific NPC's events
        private void UnsubscribeFromNPC(GameObject npc)
        {
            if (npc == null) return;

            if (npcHealthMap.TryGetValue(npc, out Behavior.NPCHealth npcHealth))
            {
                if (npcHealth != null)
                {
                    npcHealth.OnNPCDestroyed -= HandleNPCDestroyed;
                }
                npcHealthMap.Remove(npc);
            }
        }

        // Unsubscribe from all NPCs (cleanup)
        private void UnsubscribeAllNPCs()
        {
            foreach (var kvp in npcHealthMap)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.OnNPCDestroyed -= HandleNPCDestroyed;
                }
            }
            npcHealthMap.Clear();
        }

        // Get all active NPCs
        public List<GameObject> GetActiveNPCs() => new List<GameObject>(activeNPCs);

        // Clear all NPCs (useful for scene transitions)
        public void ClearAllNPCs()
        {
            // Unsubscribe from all events
            UnsubscribeAllNPCs();

            // Destroy all NPCs
            for (int i = activeNPCs.Count - 1; i >= 0; i--)
            {
                if (activeNPCs[i] != null)
                {
                    Destroy(activeNPCs[i]);
                }
            }
            
            activeNPCs.Clear();
            npcsPendingRespawn.Clear();

            // Stop respawn processor
            if (respawnProcessor != null)
            {
                StopCoroutine(respawnProcessor);
                respawnProcessor = null;
            }
        }

        // Save/Load implementation for ISaveable interface
        public string GetSaveID() => "NPCManager";

        public SaveData SaveState()
        {
            NPCManagerSaveData saveData = new NPCManagerSaveData
            {
                saveID = GetSaveID(),
                dataType = typeof(NPCManagerSaveData).Name,
                activeNPCCount = activeNPCs.Count,
                maxNPCCount = maxNPCCount,
                spawnRadius = spawnRadius,
                useSpawnCenter = useSpawnCenter,
                spawnCenter = spawnCenter
            };
            return saveData;
        }

        public void LoadState(SaveData data)
        {
            if (data is NPCManagerSaveData npcSaveData)
            {
                maxNPCCount = npcSaveData.maxNPCCount;
                spawnRadius = npcSaveData.spawnRadius;
                useSpawnCenter = npcSaveData.useSpawnCenter;
                spawnCenter = npcSaveData.spawnCenter;

                // Clear existing NPCs
                ClearAllNPCs();

                // Spawn NPCs to match saved state
                int npcsToSpawn = Mathf.Min(npcSaveData.activeNPCCount, maxNPCCount);
                for (int i = 0; i < npcsToSpawn; i++)
                {
                    SpawnNPC();
                }
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

        Core.SaveData ISaveable.SaveState()
        {
            throw new NotImplementedException();
        }

        public void LoadState(Core.SaveData data)
        {
            throw new NotImplementedException();
        }
    }
}

