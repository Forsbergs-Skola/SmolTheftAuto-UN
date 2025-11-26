using UnityEngine;
using Vehicles;   // for VehicleMover

[RequireComponent(typeof(VehicleMover))]

public class AIVehicleController : MonoBehaviour
{
    [Header("Path / Road")]
    [Tooltip("Waypoints the car will follow, in order, placed along the road.")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waypointReachDistance = 3f;
    [Tooltip("If true, picks a random next waypoint instead of just (index+1).")]
    [SerializeField] private bool randomizeNextWaypoint = false;

    [Header("Driving Style")]
    [Range(0f, 1f)]
    [SerializeField] private float baseThrottle = 0.8f;
    [Range(0f, 1f)]
    [SerializeField] private float slowDownThrottle = 0.3f;
    [Tooltip("How much sharp turns reduce throttle (0 = ignore turns, 1 = big slowdown).")]
    [Range(0f, 1f)]
    [SerializeField] private float turnSlowdownFactor = 0.4f;

    [Header("Player Avoidance / Steal Zone")]
    [Tooltip("Tag used to find the player once at Start.")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float slowDownDistanceToPlayer = 10f;
    [SerializeField] private float stopDistanceToPlayer     = 4f;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    private VehicleMover mover;
    private Transform player;
    private int currentWaypointIndex;
    private bool aiEnabled = true;

    // NOTE: This controller is intentionally lightweight and deterministic To keep AI Driver Looking Like he's Having Fun while Driving Sometimes
    

    private void Awake()
    {
        
        mover = GetComponent<VehicleMover>();
    }

    private void Start()
    {
        // Find player by tag 
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Randomize starting waypoint if we have any
        if (waypoints != null && waypoints.Length > 0)
        {
            currentWaypointIndex = Random.Range(0, waypoints.Length);
        }
    }

    private void FixedUpdate()
    {
        // Main AI loop (FixedUpdate for consistent physics-driven control):
        
        if (!aiEnabled || mover == null || waypoints == null || waypoints.Length == 0)
        {
            // no AI control
            mover.SetInput(0f, 0f, false);
            return;
        }

        Transform targetWp = waypoints[currentWaypointIndex];
        if (targetWp == null)
        {
            mover.SetInput(0f, 0f, true);
            return;
        }

        Vector3 targetPos = targetWp.position;
        Vector3 toTarget = targetPos - transform.position;
        toTarget.y = 0f;

        float distanceToWaypoint = toTarget.magnitude;

        // If close enough, choose next waypoint
        if (distanceToWaypoint < waypointReachDistance)
        {
            if (randomizeNextWaypoint)
            {
                currentWaypointIndex = Random.Range(0, waypoints.Length);
            }
            else
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            return;
        }

        // Convert target into local space for steering
        Vector3 localTarget = transform.InverseTransformPoint(targetPos);
        float steer = 0f;
        if (localTarget.magnitude > 0.001f)
        {
            steer = Mathf.Clamp(localTarget.x / localTarget.magnitude, -1f, 1f);
        }

        float throttle = baseThrottle;
        bool brake = false;

        // Slow down on sharp turns: compute angle to target and reduce throttle
        // proportionally to turn sharpness (so we don't go too fast into corners)
        float angleToTarget = Vector3.SignedAngle(transform.forward, toTarget.normalized, Vector3.up);
        float sharpness = Mathf.Clamp01(Mathf.Abs(angleToTarget) / 90f); 
        throttle *= Mathf.Clamp01(1f - sharpness * turnSlowdownFactor);

        // Player proximity handling: when the player is near, slow or stop
        // so the player can enter the vehicle or avoid collisions.
        if (player != null)
        {
            float playerDist = Vector3.Distance(transform.position, player.position);

            if (playerDist <= stopDistanceToPlayer)
            {
                // Full stop and brake so player can get in
                throttle = 0f;
                brake = true;
            }
            else if (playerDist <= slowDownDistanceToPlayer)
            {
                // Soft slow down
                throttle = Mathf.Min(throttle, slowDownThrottle);
            }
        }

        mover.SetInput(throttle, steer, brake);
    }

    
    
    /// Enable or disable AI control of this vehicle.
    /// When disabled the vehicle will brake and release control so a player
    /// or other system can take over safely.
    
    
    public void SetAIEnabled(bool enabled)
    {
        aiEnabled = enabled;
        if (!aiEnabled)
        {
            // Release control
            mover.SetInput(0f, 0f, true);
        }
    }

    private void OnDrawGizmosSelected() // Draw waypoints and path in editor when selected
    {
        if (!drawGizmos || waypoints == null) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            Gizmos.DrawSphere(waypoints[i].position, 0.5f);

            int next = (i + 1) % waypoints.Length;
            if (waypoints.Length > 1 && waypoints[next] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[next].position);
            }
        }
    }
}
