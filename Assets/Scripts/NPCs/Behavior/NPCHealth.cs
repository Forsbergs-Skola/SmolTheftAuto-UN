using UnityEngine;
using SmolTheftAuto.Core;
using SmolTheftAuto.NPCs.Data;
using Events;

namespace SmolTheftAuto.NPCs.Behavior
{
    // Component responsible for NPC health management
    // Implements IDamageable for consistent damage handling across systems
    public class NPCHealth : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        [Header("Event Channels")]
        [SerializeField] private Vector2PayloadEvent healthChangedEvent;
        [SerializeField] private EmptyPayloadEvent npcDestroyedEvent;
        [SerializeField] private GameObjectPayloadEvent npcDestroyedGameObjectEvent;

        // Event for NPCManager to track destroyed NPCs
        public System.Action<GameObject> OnNPCDestroyed;

        private NPCData npcData;
        private NPCLoot npcLoot;
        private bool isDestroyed = false;

        private void Awake()
        {
            npcLoot = GetComponent<NPCLoot>();
            
            // Try to get NPCData from parent NPCController
            var npcController = GetComponent<NPCController>();
            if (npcController != null)
            {
                npcData = npcController.GetNPCData();
            }

            // Use NPCData maxHealth if available
            if (npcData != null)
            {
                maxHealth = npcData.maxHealth;
            }

            currentHealth = maxHealth;
        }

        // Apply damage to the NPC (from IDamageable interface)
        public void TakeDamage(float damage)
        {
            if (isDestroyed) return;

            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);

            healthChangedEvent?.TriggerEvent(new Vector2(currentHealth, maxHealth));

            if (currentHealth <= 0)
            {
                DestroyNPC();
            }
        }

        // Destroy the NPC and trigger events
        private void DestroyNPC()
        {
            if (isDestroyed) return;
            isDestroyed = true;

            // Drop money through NPCLoot component
            if (npcLoot != null)
            {
                npcLoot.DropMoney();
            }

            // Trigger events for quest system and other listeners
            npcDestroyedEvent?.TriggerEvent();
            npcDestroyedGameObjectEvent?.TriggerEvent(gameObject);

            // Notify NPCManager
            OnNPCDestroyed?.Invoke(gameObject);

            // Handle respawn through NPCManager
            var spawner = ServiceLocator.Get<INPCSpawner>();
            if (spawner != null && npcData != null && npcData.shouldRespawn)
            {
<<<<<<< Updated upstream
                HandleRespawn();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Drop money when NPC is destroyed
        private void DropMoney()
        {
            int moneyAmount = Random.Range(minMoneyDrop, maxMoneyDrop + 1);
            
            if (moneyPickupPrefab != null)
            {
                GameObject moneyPickup = Instantiate(moneyPickupPrefab, transform.position, Quaternion.identity);
                var moneyPickupScript = moneyPickup.GetComponent<MoneyPickup>();
                if (moneyPickupScript != null)
                {
                    moneyPickupScript.SetMoneyAmount(moneyAmount);
                }
            }

            moneyDroppedEvent?.TriggerEvent(moneyAmount);
        }

        // Handle NPC respawn at a random location
        private void HandleRespawn()
        {
            INPCSpawner spawner = ServiceLocator.Get<INPCSpawner>();
            if (spawner != null)
            {
                spawner.RespawnNPC(gameObject, respawnDelay);
=======
                spawner.RespawnNPC(gameObject, npcData.respawnDelay);
>>>>>>> Stashed changes
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Reset health (useful for respawning)
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            isDestroyed = false;
            healthChangedEvent?.TriggerEvent(new Vector2(currentHealth, maxHealth));
        }

        // Getter methods
        public float GetCurrentHealth() => currentHealth;
        public float GetMaxHealth() => maxHealth;
        public bool IsDestroyed() => isDestroyed;
    }
}