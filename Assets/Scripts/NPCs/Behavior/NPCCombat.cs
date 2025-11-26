using UnityEngine;
using SmolTheftAuto.Core;
using SmolTheftAuto.NPCs.Data;

namespace SmolTheftAuto.NPCs.Behavior
{
    // Component responsible for dealing damage to the player
    // Handles damage at intervals, not every frame
    public class NPCCombat : MonoBehaviour
    {
        [Header("Combat Settings")]
        [SerializeField] private float damageToPlayer = 10f;
        [SerializeField] private float damageRange = 2f;
        [SerializeField] private float damageCooldown = 1f;

        private NPCData npcData;
        private NPCHealth npcHealth;
        private float lastDamageTime = 0f;

        private void Awake()
        {
            npcHealth = GetComponent<NPCHealth>();
            
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
                damageToPlayer = npcData.damageToPlayer;
                damageRange = npcData.damageRange;
                damageCooldown = npcData.damageCooldown;
            }
        }

        private void Update()
        {
            if (npcHealth != null && npcHealth.IsDestroyed())
            {
                return;
            }

            if (PlayerReference.PlayerTransform != null)
            {
                CheckPlayerDamage();
            }
        }

        // Check if player is in range and deal damage at intervals
        private void CheckPlayerDamage()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerReference.PlayerTransform.position);

            if (distanceToPlayer <= damageRange && Time.time >= lastDamageTime + damageCooldown)
            {
                DealDamageToPlayer();
                lastDamageTime = Time.time;
            }
        }

        // Deal damage to the player
        private void DealDamageToPlayer()
        {
            IDamageable damageable = PlayerReference.GetPlayerComponent<IDamageable>();
            damageable?.TakeDamage(damageToPlayer);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, damageRange);
        }
    }
}

