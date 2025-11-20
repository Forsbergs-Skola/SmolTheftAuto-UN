using UnityEngine;
using SmolTheftAuto.Core;
using SmolTheftAuto.Data;
using Events;

namespace SmolTheftAuto.NPCs.Behavior
{
    // Handles NPC health, damage, and destruction
    // NPCs can be damaged and destroyed, and drop money when destroyed
    public class NPCHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        [Header("Money Drop Settings")]
        [SerializeField] private int minMoneyDrop = 10;
        [SerializeField] private int maxMoneyDrop = 50;
        [SerializeField] private GameObject moneyPickupPrefab;

        [Header("Respawn Settings")]
        [SerializeField] private bool shouldRespawn = true;
        [SerializeField] private float respawnDelay = 5f;

        [Header("Event Channels")]
        [SerializeField] private Vector2PayloadEvent healthChangedEvent;
        [SerializeField] private EmptyPayloadEvent npcDestroyedEvent;
        [SerializeField] private GameObjectPayloadEvent npcDestroyedGameObjectEvent;
        [SerializeField] private IntPayloadEvent moneyDroppedEvent;

        private bool isDestroyed = false;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        // Apply damage to the NPC
        public void TakeDamage(float damage)
        {
            if (isDestroyed) return;

            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);

            healthChangedEvent?.TriggerEvent(payload: new Vector2(currentHealth, maxHealth));

            if (currentHealth <= 0)
            {
                DestroyNPC();
            }
        }

        // Destroy the NPC and handle money drop and respawn
        private void DestroyNPC()
        {
            if (isDestroyed) return;
            isDestroyed = true;

            DropMoney();
            npcDestroyedEvent?.TriggerEvent();
            npcDestroyedGameObjectEvent?.TriggerEvent(gameObject);

            if (shouldRespawn)
            {
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
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ResetHealth()
        {
            currentHealth = maxHealth;
            isDestroyed = false;
            healthChangedEvent?.TriggerEvent(payload: new Vector2(currentHealth, maxHealth));
        }

        public float GetCurrentHealth() => currentHealth;
        public float GetMaxHealth() => maxHealth;
        public bool IsDestroyed() => isDestroyed;
    }
}