using UnityEngine;
using SmolTheftAuto.Core;
using Events;

namespace SmolTheftAuto.Managers
{
    // Manages player's money. Handles adding/removing money and notifies UI
    public class PlayerMoney : MonoBehaviour, IMoneyReceiver
    {
        [Header("Money Settings")]
        [SerializeField] private int currentMoney = 0;

        [Header("Event Channels")]
        [SerializeField] private IntPayloadEvent moneyChangedEvent;

        // Add money to player
        public void AddMoney(int amount)
        {
            currentMoney += amount;
            moneyChangedEvent?.TriggerEvent(currentMoney);
        }

        // Remove money from player
        public bool RemoveMoney(int amount)
        {
            if (currentMoney >= amount)
            {
                currentMoney -= amount;
                moneyChangedEvent?.TriggerEvent(currentMoney);
                return true;
            }
            return false;
        }

        // Get current money amount
        public int GetMoney() => currentMoney;

        // Set money amount (for save/load)
        public void SetMoney(int amount)
        {
            currentMoney = amount;
            moneyChangedEvent?.TriggerEvent(currentMoney);
        }
    }
}


