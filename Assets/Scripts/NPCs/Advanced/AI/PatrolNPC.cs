using UnityEngine;
using UnityEngine.AI;

namespace SmolTheftAuto.NPCs.Advanced.AI
{
    /// <summary>
    /// Type A: Patrol NPC. Walks along predefined checkpoint paths on sidewalks.
    /// Non-aggressive, ignores player unless attacked.
    /// </summary>
    public class PatrolNPC : MonoBehaviour
    {
        [Header("Patrol Settings")]
        [SerializeField] private CheckpointPath patrolPath;
        [SerializeField] private float stoppingDistance = 0.5f;
        [SerializeField] private float patrolSpeed = 3.5f;

        [Header("Behavior")]
        [SerializeField] private bool idleAtCheckpoints = true;

        // State
        private NavMeshAgent navMeshAgent;
        private Animator animator;
        private int currentWaypointIndex = 0;
        private float checkpointPauseTimer = 0f;
        private bool isPaused = false;

        // Animator parameter names
        private const string SPEED_PARAM = "Speed";
        private const string IS_WALKING = "IsWalking";

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            if (navMeshAgent == null)
            {
                Debug.LogError($"{nameof(PatrolNPC)} requires NavMeshAgent component.", this);
            }

            if (patrolPath == null)
            {
                Debug.LogError($"{nameof(PatrolNPC)} requires a {nameof(CheckpointPath)} assigned.", this);
            }
        }

        private void Start()
        {
            if (patrolPath != null && navMeshAgent != null)
            {
                // Set NavMeshAgent speed
                navMeshAgent.speed = patrolSpeed;
                navMeshAgent.stoppingDistance = stoppingDistance;

                // Move to first waypoint
                MoveToNextWaypoint();
            }
        }

        private void Update()
        {
            if (navMeshAgent == null || patrolPath == null)
                return;

            // NEW: Handle pausing at checkpoints
            if (isPaused)
            {
                checkpointPauseTimer -= Time.deltaTime;
                if (checkpointPauseTimer <= 0)
                {
                    isPaused = false;
                    MoveToNextWaypoint();
                }
                return;
            }

            // Check if reached current waypoint
            if (HasReachedWaypoint())
            {
                if (idleAtCheckpoints)
                {
                    StartCheckpointPause();
                }
                else
                {
                    MoveToNextWaypoint();
                }
            }

            // NEW: Update animator with current speed
            UpdateAnimator();
        }

        /// <summary>
        /// Check if NPC has reached current waypoint.
        /// </summary>
        private bool HasReachedWaypoint()
        {
            if (navMeshAgent.pathPending)
                return false;

            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                return false;

            if (navMeshAgent.hasPath && navMeshAgent.velocity.sqrMagnitude > 0.2f)
                return false;

            return true;
        }

        /// <summary>
        /// Start pause at current checkpoint.
        /// </summary>
        private void StartCheckpointPause()
        {
            isPaused = true;
            checkpointPauseTimer = patrolPath.GetPauseTime();
            
            // Stop movement
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.SetDestination(transform.position);

            if (animator != null)
            {
                animator.SetBool(IS_WALKING, false);
            }

            Debug.Log($"NPC paused at checkpoint for {checkpointPauseTimer} seconds");
        }

        /// <summary>
        /// Move NPC to next waypoint in path.
        /// </summary>
        private void MoveToNextWaypoint()
        {
            if (patrolPath == null) return;

            Transform nextWaypoint = patrolPath.GetWaypoint(currentWaypointIndex);
            if (nextWaypoint == null)
            {
                Debug.LogWarning("Could not get waypoint. Path may be incomplete.", this);
                return;
            }

            // NEW: Set destination for NavMeshAgent
            navMeshAgent.SetDestination(nextWaypoint.position);

            // Update animator
            if (animator != null)
            {
                animator.SetBool(IS_WALKING, true);
            }

            Debug.Log($"NPC moving to waypoint {currentWaypointIndex}");

            // Move to next waypoint index
            currentWaypointIndex = patrolPath.GetNextWaypointIndex(currentWaypointIndex);
        }

        /// <summary>
        /// Update animator parameters based on movement.
        /// </summary>
        private void UpdateAnimator()
        {
            if (animator == null || navMeshAgent == null)
                return;

            // Calculate current speed
            float speed = navMeshAgent.velocity.magnitude;

            // Update speed parameter for blending
            animator.SetFloat(SPEED_PARAM, speed);

            // Update walking state
            bool isWalking = speed > 0.1f && !isPaused;
            animator.SetBool(IS_WALKING, isWalking);
        }

        /// <summary>
        /// Stop patrol and freeze in place.
        /// Called when NPC is hit or needs to stop.
        /// </summary>
        public void StopPatrol()
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.velocity = Vector3.zero;
                navMeshAgent.SetDestination(transform.position);
            }

            isPaused = true;

            if (animator != null)
            {
                animator.SetBool(IS_WALKING, false);
            }
        }

        /// <summary>
        /// Resume patrol from current position.
        /// </summary>
        public void ResumePatrol()
        {
            isPaused = false;
            MoveToNextWaypoint();
        }

        private void OnDrawGizmosSelected()
        {
            // Draw destination indicator
            if (navMeshAgent != null && navMeshAgent.hasPath)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(navMeshAgent.destination, 0.5f);
            }

            // Draw current waypoint
            if (patrolPath != null)
            {
                Transform currentWaypoint = patrolPath.GetWaypoint(currentWaypointIndex);
                if (currentWaypoint != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(currentWaypoint.position, 1f);
                }
            }
        }
    }
}
