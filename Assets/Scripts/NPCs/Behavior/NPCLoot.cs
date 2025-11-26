using UnityEngine;
using SmolTheftAuto.NPCs.Data;
using SmolTheftAuto.Data;
using Events;

namespace SmolTheftAuto.NPCs.Behavior
{
    // Component responsible for dropping money when NPC dies
    // Follows single responsibility principle
    public class NPCLoot : MonoBehaviour
    {
        [Header("Money Drop Settings")]
        [SerializeField] private int minMoneyDrop = 10;
        [SerializeField] private int maxMoneyDrop = 50;
        [SerializeField] private GameObject moneyPickupPrefab;

        [Header("Event Channels")]
        [SerializeField] private IntPayloadEvent moneyDroppedEvent;

        private NPCData npcData;

        private void Awake()
        {
            // Try to get NPCData from parent NPCController
            var npcController = GetComponent<NPCController>();
            if (npcController != null)
            {
                npcData = npcController.GetNPCData();
            }
        }

        // Drop money at the NPC's position
        public void DropMoney()
        {
            // Use NPCData if available, otherwise use serialized values
            int minDrop = npcData != null ? npcData.minMoneyDrop : minMoneyDrop;
            int maxDrop = npcData != null ? npcData.maxMoneyDrop : maxMoneyDrop;
            int moneyAmount = Random.Range(minDrop, maxDrop + 1);

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
    }
}

