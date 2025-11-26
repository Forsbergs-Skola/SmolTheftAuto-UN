using SmolTheftAuto.Managers;
using SmolTheftAuto.Core;
using UnityEngine;

namespace SmolTheftAuto.Data
{
    // Handles money pickup when player collects money dropped by NPCs
    [RequireComponent(typeof(Collider))]
    public class MoneyPickup : MonoBehaviour
    {
        [Header("Pickup Settings")]
        [SerializeField] private int moneyAmount = 10;
        [SerializeField] private float pickupRange = 2f;
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private float floatSpeed = 1f;
        [SerializeField] private float floatAmount = 0.5f;

        private Vector3 startPosition;
        private Collider pickupCollider;

        private void Start()
        {
            pickupCollider = GetComponent<Collider>();
            if (pickupCollider != null)
            {
                pickupCollider.isTrigger = true;
                if (pickupCollider is SphereCollider sphereCollider)
                {
                    sphereCollider.radius = pickupRange;
                }
            }
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

<<<<<<< HEAD
<<<<<<< Updated upstream
=======
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == PlayerReference.PlayerGameObject ||
                other.CompareTag(GameConstants.Tags.PLAYER))
            {
                PickupMoney();
            }
        }

>>>>>>> Stashed changes
=======
        public void DoPickupStuff()
        {

        }

        /*

>>>>>>> main
        // Set the money amount for this pickup
        public void SetMoneyAmount(int amount)
        {
            moneyAmount = Mathf.Max(0, amount);
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


