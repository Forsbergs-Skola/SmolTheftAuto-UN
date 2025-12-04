using UnityEngine;
using SmolTheftAuto.Core;
using SmolTheftAuto.Data;
using Events;
using UnityEngine.AI;
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

        [Header("Death Behaviour")] //Reference to Disable stuff after NpC dies (ibads Addition)
        [SerializeField] private Collider[] collidersToDisable;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private MonoBehaviour[] aiScriptsToDisable; 

        private bool isDestroyed = false;
        [SerializeField] private Animator animator;

        private void Awake()
        {
            currentHealth = maxHealth;
            if (collidersToDisable == null || collidersToDisable.Length == 0)
                collidersToDisable = GetComponentsInChildren<Collider>();

            if (navMeshAgent == null)
                navMeshAgent = GetComponent<NavMeshAgent>();

            if (aiScriptsToDisable == null || aiScriptsToDisable.Length == 0)
                aiScriptsToDisable = new MonoBehaviour[]
                {
            GetComponent<SmolTheftAuto.NPCs.Advanced.AI.AggressiveNPC>(),
            GetComponent<SmolTheftAuto.NPCs.Behavior.NPCController>()
                };
        }

        // Destroy the NPC and handle money drop and respawn
        private void DestroyNPC()
        {
            if (isDestroyed) return;
            isDestroyed = true;

            // Immediately stop all movement and collision so the NPC doesn't Float around as a Deadbody and Irritates the Player from beyond the grave(ibads Addition)
            if (navMeshAgent != null)
                navMeshAgent.enabled = false;

            if (collidersToDisable != null)
            {
                foreach (var col in collidersToDisable)
                {
                    if (col != null) col.enabled = false;
                }
            }

            if (aiScriptsToDisable != null)
            {
                foreach (var script in aiScriptsToDisable)
                {
                    if (script != null) script.enabled = false;
                }
            }

            // Playing Death Animation
            if (animator != null)
                animator.SetBool("isDead", true);

            DropMoney();
            npcDestroyedEvent?.TriggerEvent();

            if (shouldRespawn)
                HandleRespawn();
            else
                Destroy(gameObject);
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
                    //moneyPickupScript.SetMoneyAmount(moneyAmount);
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

        public float Health
        {
            get => currentHealth;
            set
            {
                if (isDestroyed)
                    return;

                currentHealth = Mathf.Max(0, value);
                healthChangedEvent?.TriggerEvent(new Vector2(currentHealth, maxHealth));

                if (currentHealth <= 0)
                    DestroyNPC();
            }
        }



        public float GetCurrentHealth() => currentHealth;
        public float GetMaxHealth() => maxHealth;
        public bool IsDestroyed() => isDestroyed;
    }
}