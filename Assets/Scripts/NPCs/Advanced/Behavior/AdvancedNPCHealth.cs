using UnityEngine;
using SmolTheftAuto.Core;
using SmolTheftAuto.Data;
using Events;

namespace SmolTheftAuto.NPCs.Advanced.Behavior
{
   
    /// Enhanced NPC Health system for the advanced NPC framework.
    /// Handles death sequence: freeze movement → drop money → play animation → ragdoll/cleanup.
  
    public class AdvancedNPCHealth : MonoBehaviour
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

        // NEW: References for death handling
        private Physics.RagdollSetup ragdollSetup;
        private Animator animator;
        private UnityEngine.AI.NavMeshAgent navMeshAgent;

        private bool isDestroyed = false;

        private void Awake()
        {
            currentHealth = maxHealth;
            ragdollSetup = GetComponent<Physics.RagdollSetup>();
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

            if (ragdollSetup == null)
            {
                Debug.LogWarning($"{nameof(AdvancedNPCHealth)} works best with {nameof(Physics.RagdollSetup)}.", this);
            }
        }

        /// Apply damage to NPC. Triggers death sequence if health <= 0.
      
        public void TakeDamage(float damage, bool isWeaponDamage = true)
        {
            if (isDestroyed) return;

            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);

            healthChangedEvent?.TriggerEvent(payload: new Vector2(currentHealth, maxHealth));

            if (currentHealth <= 0)
            {
                DestroyNPC(isWeaponDamage);
            }
        }


        /// Main death sequence for NPC:
        /// 1. Freeze movement immediately
        /// 2. Drop money
        /// 3. Play death animation
        /// 4. Enable ragdoll after animation completes

        private void DestroyNPC(bool isWeaponDamage = true)
        {
            if (isDestroyed) return;
            isDestroyed = true;

            // NEW: STEP 1 - Freeze movement IMMEDIATELY
            if (ragdollSetup != null)
            {
                ragdollSetup.FreezeMovement();
            }
            else if (navMeshAgent != null && navMeshAgent.enabled)
            {
                // MODIFIED: Fallback if no ragdoll system
                navMeshAgent.enabled = false;
            }

            // NEW: STEP 2 - Drop money IMMEDIATELY
            DropMoney();

            // NEW: STEP 3 - Play death animation
            PlayDeathAnimation(isWeaponDamage);

            // NEW: STEP 4 - Handle respawn or destruction
            npcDestroyedEvent?.TriggerEvent();

            if (shouldRespawn)
            {
                HandleRespawn();
            }
            else
            {
                // Schedule destruction after animation completes (if animator exists)
                if (animator != null)
                {
                    // Estimate animation length - adjust based on your death animation
                    float animationLength = 2f; // Default 2 seconds
                    Destroy(gameObject, animationLength);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        /// Drop money immediately upon death (before animation).
    
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

        
        /// Play appropriate death animation based on death cause.
        /// NEW: Supports different animations for weapon vs vehicle deaths.
        
        private void PlayDeathAnimation(bool isWeaponDamage)
        {
            // NEW: Play death animation if animator is available
            if (animator != null && animator.isActiveAndEnabled)
            {
                if (isWeaponDamage)
                {
                    // Play weapon death animation (standing, falling forward/backward)
                    animator.SetTrigger("Die");
                    // OPTIONAL: animator.SetInteger("DeathType", 0); // For different death anims
                }
                else
                {
                    // Play vehicle impact death animation (ragdoll-like)
                    animator.SetTrigger("Die");
                    // OPTIONAL: animator.SetInteger("DeathType", 1); // For impact anims
                }

                Debug.Log($"Playing death animation. Weapon damage: {isWeaponDamage}");
            }
        }

       
        /// Handle NPC respawn at a random location.
      
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

     
        /// Reset health and state (called on respawn).
      
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            isDestroyed = false;
            healthChangedEvent?.TriggerEvent(payload: new Vector2(currentHealth, maxHealth));

            // NEW: Reset ragdoll state
            if (ragdollSetup != null)
            {
                ragdollSetup.DisableRagdoll();
                ragdollSetup.ResumeMovement();
            }

            // Reset animator
            if (animator != null)
            {
                animator.ResetTrigger("Die");
            }
        }

        public float Health
        {
            get => currentHealth;
            set
            {
                if (isDestroyed) return;
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
