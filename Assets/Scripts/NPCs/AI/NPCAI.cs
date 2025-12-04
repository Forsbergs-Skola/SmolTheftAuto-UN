using UnityEngine;
using UnityEngine.AI;

namespace SmolTheftAuto.NPCs.AI
{
    // Basic AI behavior for NPCs using NavMesh for wandering
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCAI : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private float wanderRadius = 10f;
        [SerializeField] private float wanderTimer = 5f;
        [SerializeField] private bool useNavMesh = true;
        
        [Header("Animation Settings")]
        [SerializeField] private Animator animator;
        

        private NavMeshAgent navAgent;
        private float timer;
        private Vector3 startPosition;

        private void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
            startPosition = transform.position;
        }

        private void Start()
        {
            if (useNavMesh && navAgent != null)
            {
                navAgent.enabled = true;
            }
        }

        private void Update()
        {
            if (!useNavMesh || navAgent == null || !navAgent.enabled) return;
            
            float speedPercent = navAgent.velocity.magnitude / navAgent.speed;
            animator.SetFloat("moveSpeed", speedPercent);

            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                Vector3 newPos = GetRandomPosition();
                if (navAgent.isOnNavMesh)
                {
                    navAgent.SetDestination(newPos);
                }
                timer = 0;
            }
        }

        private Vector3 GetRandomPosition()
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += startPosition;
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
            {
                return hit.position;
            }
            
            return startPosition;
        }

        public void SetStartPosition(Vector3 position)
        {
            startPosition = position;
        }
    }
}


