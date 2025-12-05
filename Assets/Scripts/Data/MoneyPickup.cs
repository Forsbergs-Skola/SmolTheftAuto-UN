using SmolTheftAuto.Managers;
using SmolTheftAuto.Core;
using Events;
using UnityEngine;

namespace SmolTheftAuto.Data
{
    // Handles money pickup when player collects money dropped by NPCs
    public class MoneyPickup : MonoBehaviour
    {
        [Header("Pickup Settings")]
        [SerializeField] private int moneyAmount = 10;
        [SerializeField] private float pickupRange = 2f;
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private float floatSpeed = 1f;
        [SerializeField] private float floatAmount = 0.5f;

        [SerializeField] private IntPayloadEvent moneyEvent;

        private bool collectible = true;

        private Vector3 startPosition;

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmount;

            /*
            if (PlayerReference.PlayerTransform != null)
            {
                float distance = Vector3.Distance(transform.position, PlayerReference.PlayerTransform.position);
                if (distance <= pickupRange)
                {
                    PickupMoney();
                }
            }
            */
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!collectible) return;
                collectible = false;

                AudioManager.Instance.PickupSfx();

                moneyEvent.TriggerEvent(moneyAmount);
                Destroy(gameObject);

            }
        }

        public void DoPickupStuff()
        {

        }

        /*

        // Set the money amount for this pickup
        public void SetMoneyAmount(int amount)
        {
            moneyAmount = amount;
        }

        private void PickupMoney()
        {
            IMoneyReceiver moneyReceiver = PlayerReference.GetPlayerComponent<IMoneyReceiver>();
            if (moneyReceiver != null)
            {
                moneyReceiver.AddMoney(moneyAmount);
            }

            Destroy(gameObject);
        }
        */

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, pickupRange);
        }
    }
}


