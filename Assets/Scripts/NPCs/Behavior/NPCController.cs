using UnityEngine;
using SmolTheftAuto.NPCs.Data;

namespace SmolTheftAuto.NPCs.Behavior
{
    // Main controller that coordinates all NPC components
    // Acts as a coordinator rather than handling all logic itself
    [RequireComponent(typeof(NPCHealth))]
    public class NPCController : MonoBehaviour
    {
        [Header("NPC Data")]
        [SerializeField] private NPCData npcData;

        // Component references
        private NPCHealth npcHealth;
        private NPCCombat npcCombat;
        private NPCMovement npcMovement;
        private NPCLoot npcLoot;

        private void Awake()
        {
            // Get all component references
            npcHealth = GetComponent<NPCHealth>();
            npcCombat = GetComponent<NPCCombat>();
            npcMovement = GetComponent<NPCMovement>();
            npcLoot = GetComponent<NPCLoot>();

            // Validate required components
            if (npcHealth == null)
            {
                Debug.LogError($"{nameof(NPCController)} requires {nameof(NPCHealth)} component!", this);
            }
        }

        // Get the NPCData assigned to this NPC
        public NPCData GetNPCData() => npcData;

        // Set NPCData (useful for runtime configuration)
        public void SetNPCData(NPCData data)
        {
            npcData = data;
        }

        // Get component references (for external access if needed)
        public NPCHealth GetNPCHealth() => npcHealth;
        public NPCCombat GetNPCCombat() => npcCombat;
        public NPCMovement GetNPCMovement() => npcMovement;
        public NPCLoot GetNPCLoot() => npcLoot;
    }
}
