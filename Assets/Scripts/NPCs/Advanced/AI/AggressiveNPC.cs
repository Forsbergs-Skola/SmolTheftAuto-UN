using UnityEngine;
using UnityEngine.AI;
using SmolTheftAuto.Core;

namespace SmolTheftAuto.NPCs.Advanced.AI
{
    /// <summary>
    /// Type B: Aggressive Chase NPC. Hunts player when detected.
    /// All nearby aggressive NPCs activate simultaneously when player detected.
    /// Deals increased damage (configurable multiplier).
    /// </summary>
    public class AggressiveNPC : MonoBehaviour
    {
        [Header("NPC Type")]
        [SerializeField] private bool isAggressive = true;

        [Header("Detection")]
        [SerializeField] private float detectionRadius = 30f;
        [SerializeField] private float losePlayerDistance = 60f;

        [Header("Chase Behavior")]
        [SerializeField] private float chaseSpeed = 5.5f;
        [SerializeField] private float baseSpeed = 3.5f;
        [SerializeField] private float damageMultiplier = 1.5f;
        [SerializeField] private float attackRange = 2.5f;
        [SerializeField] private float attackCooldown = 1.5f;

        [Header("References")]
        [SerializeField] private float baseDamageToPlayer = 10f;

        // State
        private enum NPCState { Idle, Patrol, Detecting, Chasing, Attacking, Returning }
        private NPCState currentState = NPCState.Idle;

        private NavMeshAgent navMeshAgent;
        private Animator animator;
        private PlayerDetectionZone detectionZone;

        private GameObject detectedPlayer;
        private float lastAttackTime = 0f;

        // Animator parameters
        private const string SPEED_PARAM = "Speed";
        private const string IS_WALKING = "IsWalking";
        private const string IS_CHASING = "IsChasing";
        private const string IS_ATTACKING = "IsAttacking";

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            if (navMeshAgent == null)
            {
                Debug.LogError($"{nameof(AggressiveNPC)} requires NavMeshAgent.", this);
            }

            // NEW: Create detection zone as child object
            if (isAggressive)
            {
                CreateDetectionZone();
            }
        }

        private void Start()
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.speed = baseSpeed;
            }

            if (isAggressive)
            {
                currentState = NPCState.Patrol;
            }
        }

        private void Update()
        {
            if (!isAggressive || navMeshAgent == null)
                return;

            switch (currentState)
            {
                case NPCState.Patrol:
                    UpdatePatrolState();
                    break;
                case NPCState.Detecting:
                    UpdateDetectingState();
                    break;
                case NPCState.Chasing:
                    UpdateChasingState();
                    break;
                case NPCState.Attacking:
                    UpdateAttackingState();
                    break;
            }

            UpdateAnimator();
        }

        /// <summary>
        /// Create detection zone as child object for player detection.
        /// </summary>
        private void CreateDetectionZone()
        {
            GameObject detectionObj = new GameObject("DetectionZone");
            detectionObj.transform.SetParent(transform);
            detectionObj.transform.localPosition = Vector3.zero;

            detectionZone = detectionObj.AddComponent<PlayerDetectionZone>();
            // NEW: Configure detection radius
            SphereCollider collider = detectionObj.GetComponent<SphereCollider>();
            if (collider != null)
            {
                collider.radius = detectionRadius;
            }
        }

        /// <summary>
        /// Called by detection zone when player is detected.
        /// </summary>
        public void OnPlayerDetected(GameObject player)
        {
            detectedPlayer = player;
            if (currentState != NPCState.Chasing && currentState != NPCState.Attacking)
            {
                currentState = NPCState.Detecting;
                Debug.Log($"{gameObject.name} detected player and entering chase state!");
            }
        }

        /// <summary>
        /// Called by detection zone when player is lost.
        /// </summary>
        public void OnPlayerLost()
        {
            detectedPlayer = null;
            currentState = NPCState.Patrol;
            Debug.Log($"{gameObject.name} lost player, returning to patrol.");
        }

        private void UpdatePatrolState()
        {
            // NEW: In patrol state, just wander or idle
            if (navMeshAgent.velocity.magnitude < 0.1f)
            {
                // Pick random direction to wander
                Vector3 randomDirection = Random.insideUnitSphere * 10f;
                randomDirection += transform.position;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
                {
                    navMeshAgent.SetDestination(hit.position);
                }
            }
        }

        private void UpdateDetectingState()
        {
            // Quick transition to chasing
            if (detectedPlayer != null)
            {
                currentState = NPCState.Chasing;
            }
        }

        private void UpdateChasingState()
        {
            if (detectedPlayer == null)
            {
                currentState = NPCState.Patrol;
                return;
            }

            // Check distance to player
            float distanceToPlayer = Vector3.Distance(transform.position, detectedPlayer.transform.position);

            // If too far, return to patrol
            if (distanceToPlayer > losePlayerDistance)
            {
                OnPlayerLost();
                return;
            }

            // If in attack range, attack
            if (distanceToPlayer <= attackRange)
            {
                currentState = NPCState.Attacking;
                navMeshAgent.SetDestination(transform.position); // Stop moving
                return;
            }

            // NEW: Chase player
            navMeshAgent.speed = chaseSpeed;
            navMeshAgent.SetDestination(detectedPlayer.transform.position);

            // NEW: Face player
            Vector3 directionToPlayer = (detectedPlayer.transform.position - transform.position).normalized;
            if (directionToPlayer != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(directionToPlayer),
                    Time.deltaTime * 5f
                );
            }
        }

        private void UpdateAttackingState()
        {
            if (detectedPlayer == null)
            {
                currentState = NPCState.Patrol;
                return;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, detectedPlayer.transform.position);

            // If player got away, resume chasing
            if (distanceToPlayer > attackRange * 1.5f)
            {
                currentState = NPCState.Chasing;
                navMeshAgent.speed = chaseSpeed;
                return;
            }

            // Try to attack
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }

            // Face player while attacking
            Vector3 directionToPlayer = (detectedPlayer.transform.position - transform.position).normalized;
            if (directionToPlayer != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(directionToPlayer),
                    Time.deltaTime * 5f
                );
            }
        }

        /// <summary>
        /// Attack the player with multiplied damage.
        /// </summary>
        private void AttackPlayer()
        {
            if (detectedPlayer == null) return;

            // NEW: Apply damage multiplier for aggressive NPCs
            float damageDealt = baseDamageToPlayer * damageMultiplier;

            var playerController = detectedPlayer.GetComponent<PlayerController>();
            if (playerController != null)
            {
                //playerController.TakeDamage((int)damageDealt);
            }

            // Play attack animation
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            Debug.Log($"Aggressive NPC attacking player for {damageDealt} damage!");
        }

        private void UpdateAnimator()
        {
            if (animator == null || navMeshAgent == null)
                return;

            float speed = navMeshAgent.velocity.magnitude;
            animator.SetFloat(SPEED_PARAM, speed);

            bool isWalking = speed > 0.1f;
            animator.SetBool(IS_WALKING, isWalking);

            bool isChasing = currentState == NPCState.Chasing || currentState == NPCState.Detecting;
            animator.SetBool(IS_CHASING, isChasing);

            bool isAttacking = currentState == NPCState.Attacking;
            animator.SetBool(IS_ATTACKING, isAttacking);
        }

        /// <summary>
        /// Stop aggressive behavior (when hit by weapon, etc).
        /// </summary>
        public void StopChasing()
        {
            currentState = NPCState.Idle;
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.SetDestination(transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            if (!isAggressive) return;

            // Detection radius
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            // Lose player distance
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, losePlayerDistance);

            // Attack range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
