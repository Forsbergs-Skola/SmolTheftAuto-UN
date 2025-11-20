using UnityEngine;
using SmolTheftAuto.Core;
using Events;

namespace SmolTheftAuto.Player.Health
{
    // Handles player health, damage, and regeneration
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        [Header("Regeneration Settings")]
        [SerializeField] private float regenRate = 5f;
        [SerializeField] private float regenDelay = 3f;

        [Header("Event Channels")]
        [SerializeField] private Vector2PayloadEvent healthChangedEvent;
        [SerializeField] private EmptyPayloadEvent playerDestroyedEvent;

        private float lastDamageTime = 0f;
        private bool isDestroyed = false;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        private void Update()
        {
            if (isDestroyed) return;

            if (Time.time >= lastDamageTime + regenDelay && currentHealth < maxHealth)
            {
                RegenerateHealth();
            }
        }

        // Apply damage to the player
        public void TakeDamage(float damage)
        {
            if (isDestroyed) return;

            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);
            lastDamageTime = Time.time;

            healthChangedEvent?.TriggerEvent(payload: new Vector2(currentHealth, maxHealth));

            if (currentHealth <= 0)
            {
                DestroyPlayer();
            }
        }

        // Regenerate health over time
        private void RegenerateHealth()
        {
            currentHealth += regenRate * Time.deltaTime;
            currentHealth = Mathf.Min(maxHealth, currentHealth);
            healthChangedEvent?.TriggerEvent(payload: new Vector2(currentHealth, maxHealth));
        }

        // Destroy the player and trigger death event
        private void DestroyPlayer()
        {
            if (isDestroyed) return;
            isDestroyed = true;

            playerDestroyedEvent?.TriggerEvent();
        }

        public float GetCurrentHealth() => currentHealth;
        public float GetMaxHealth() => maxHealth;
        public bool IsDestroyed() => isDestroyed;
    }
}


