using UnityEngine;
using SmolTheftAuto.Core;

namespace SmolTheftAuto.NPCs.Behavior
{
    // Main controller for NPC behavior. Handles NPC state and basic interactions
    [RequireComponent(typeof(NPCHealth))]
    public class NPCController : MonoBehaviour
    {
        [Header("NPC Settings")]
        [SerializeField] private float damageToPlayer = 10f;
        [SerializeField] private float damageRange = 2f;
        [SerializeField] private float damageCooldown = 1f;
        
        
        [SerializeField]  private Animator animator;
        
        private NPCHealth npcHealth;
        private float lastDamageTime = 0f;
       
        private void Awake()
        {
            npcHealth = GetComponent<NPCHealth>();
            if (npcHealth == null)
            {
                Debug.LogError($"{nameof(NPCController)} requires {nameof(NPCHealth)} on the same GameObject.", this);
            }
        }

        private void Update()
        {
            // if (PlayerReference.PlayerTransform != null && !npcHealth.IsDestroyed())
            // {
            //     //CheckPlayerDamage();
            // }
        }

        // private void CheckPlayerDamage()
        // {
        //     float distanceToPlayer = Vector3.Distance(transform.position, PlayerReference.PlayerTransform.position);

        //     if (distanceToPlayer <= damageRange && Time.time >= lastDamageTime + damageCooldown)
        //     {
        //         DealDamageToPlayer();
        //         lastDamageTime = Time.time;
        //     }
        // }
        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                //DealDamageToPlayer();
                other.gameObject.GetComponent<PlayerController>().TakeDamage();
                AttackAnimations();
            }
        }

        private void AttackAnimations()
        {
            int random = Random.Range(0, 4);
            float blend = random / 3f;
            animator.SetFloat("attackNumber", blend);
            animator.SetTrigger("attack");
        }
        
        
        
        // private void DealDamageToPlayer()
        // {
        //     IDamageable damageable = PlayerReference.GetPlayerComponent<IDamageable>();
        //     damageable?.TakeDamage(damageToPlayer);
        // }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, damageRange);
        }
    }
}


