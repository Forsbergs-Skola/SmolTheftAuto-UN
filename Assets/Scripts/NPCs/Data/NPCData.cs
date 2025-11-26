using UnityEngine;

namespace SmolTheftAuto.NPCs.Data
{
    // ScriptableObject for configurable NPC properties
    // Allows designers to create different NPC types with different stats
    [CreateAssetMenu(fileName = "NPCData", menuName = "Smol Theft Auto/NPC Data")]
    public class NPCData : ScriptableObject
    {
        [Header("Health Settings")]
        public float maxHealth = 100f;

        [Header("Combat Settings")]
        public float damageToPlayer = 10f;
        public float damageRange = 2f;
        public float damageCooldown = 1f;

        [Header("Movement Settings")]
        public float movementSpeed = 3f;
        public float wanderRadius = 10f;
        public float wanderTimer = 5f;

        [Header("Loot Settings")]
        public int minMoneyDrop = 10;
        public int maxMoneyDrop = 50;

        [Header("Respawn Settings")]
        public bool shouldRespawn = true;
        public float respawnDelay = 5f;
    }
}

