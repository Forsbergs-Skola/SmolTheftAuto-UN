using UnityEngine;
using System.Collections.Generic;

namespace SmolTheftAuto.NPCs.Advanced.AI
{
    /// <summary>
    /// Defines a path of checkpoints for NPCs to patrol.
    /// NPCs follow waypoints in sequence, pausing at each checkpoint.
    /// </summary>
    public class CheckpointPath : MonoBehaviour
    {
        [Header("Path Settings")]
        [SerializeField] private List<Transform> waypoints = new List<Transform>();
        [SerializeField] private bool loopPath = true;
        [SerializeField] private float pauseAtCheckpoint = 2f;

        [Header("Visualization")]
        [SerializeField] private bool showPathInEditor = true;
        [SerializeField] private float waypointSize = 0.5f;
        [SerializeField] private Color pathColor = Color.green;

        /// <summary>
        /// Get the waypoint at specified index.
        /// </summary>
        public Transform GetWaypoint(int index)
        {
            if (waypoints.Count == 0) return null;
            
            if (loopPath)
            {
                return waypoints[index % waypoints.Count];
            }
            else
            {
                return index < waypoints.Count ? waypoints[index] : null;
            }
        }

        /// <summary>
        /// Get next waypoint index. Returns -1 if path is complete (non-looping).
        /// </summary>
        public int GetNextWaypointIndex(int currentIndex)
        {
            int nextIndex = currentIndex + 1;

            if (loopPath)
            {
                return nextIndex % waypoints.Count;
            }
            else
            {
                return nextIndex < waypoints.Count ? nextIndex : -1;
            }
        }

        /// <summary>
        /// Get total number of waypoints in path.
        /// </summary>
        public int GetWaypointCount() => waypoints.Count;

        /// <summary>
        /// Get pause time at checkpoints.
        /// </summary>
        public float GetPauseTime() => pauseAtCheckpoint;

        /// <summary>
        /// Check if path loops or ends.
        /// </summary>
        public bool IsLooping() => loopPath;

        private void OnDrawGizmosSelected()
        {
            if (!showPathInEditor || waypoints.Count < 2) return;

            Gizmos.color = pathColor;

            // Draw path lines
            for (int i = 0; i < waypoints.Count; i++)
            {
                if (waypoints[i] == null) continue;

                Transform nextWaypoint = null;
                if (i < waypoints.Count - 1)
                {
                    nextWaypoint = waypoints[i + 1];
                }
                else if (loopPath && waypoints[0] != null)
                {
                    nextWaypoint = waypoints[0];
                }

                if (nextWaypoint != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, nextWaypoint.position);
                }

                // Draw waypoint spheres
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(waypoints[i].position, waypointSize);

                // Draw waypoint labels (if possible)
                Gizmos.color = pathColor;
            }

            // Highlight loop arrow if looping
            if (loopPath && waypoints.Count > 1)
            {
                Gizmos.color = new Color(pathColor.r, pathColor.g, pathColor.b, 0.5f);
                Vector3 lastPos = waypoints[waypoints.Count - 1].position;
                Vector3 firstPos = waypoints[0].position;
                Vector3 direction = (firstPos - lastPos).normalized;
                Gizmos.DrawLine(lastPos, lastPos + direction * 1f);
            }
        }

        // NEW: Editor method to add waypoints easily
#if UNITY_EDITOR
        public void AddWaypoint(Transform waypoint)
        {
            if (waypoint != null && !waypoints.Contains(waypoint))
            {
                waypoints.Add(waypoint);
            }
        }

        public void ClearWaypoints()
        {
            waypoints.Clear();
        }
#endif
    }
}
