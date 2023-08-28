using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monument.World
{
    [System.Serializable]
    public class NavNode : MonoBehaviour
    {
        [SerializeField] private float _offset = 1f;
        [SerializeField] private bool globalWalkpoint = false;
        [SerializeField] private List<NavNode> neighbors = new List<NavNode>();

#if UNITY_EDITOR
        public bool ShowNeighborsFoldout { get; set; } = false;
#endif

        public bool IsReachable { get; set; } = false;

        public List<NavNode> Neighbors => neighbors;
        
        public RotativePlatform RotativePlatform { get; set; } = null;

        public Vector3 WalkPoint
        {
            get => globalWalkpoint ?
                transform.position + (Vector3.up * 0.5f * _offset) :
                transform.position + (transform.up * 0.5f * _offset);
        }

        // Private attributes
        private Vector2[] perspectiveJoints;

        // Gizmos parameters
        private const float sphereSize = 0.16f;
        private const float cubeJointsSize = 0.125f;

        private void OnDrawGizmos()
        {
            // Draw center
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(WalkPoint, sphereSize);

            // Draw 4 line directions
            Gizmos.DrawLine(WalkPoint, WalkPoint + transform.forward * 0.5f);
            Gizmos.DrawLine(WalkPoint, WalkPoint - transform.forward * 0.5f);
            Gizmos.DrawLine(WalkPoint, WalkPoint + transform.right * 0.5f);
            Gizmos.DrawLine(WalkPoint, WalkPoint - transform.right * 0.5f);

            // Draw 4 cube joints
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(WalkPoint + transform.forward * 0.5f, Vector3.one * cubeJointsSize);
            Gizmos.DrawCube(WalkPoint - transform.forward * 0.5f, Vector3.one * cubeJointsSize);
            Gizmos.DrawCube(WalkPoint + transform.right * 0.5f, Vector3.one * cubeJointsSize);
            Gizmos.DrawCube(WalkPoint - transform.right * 0.5f, Vector3.one * cubeJointsSize);

            if (IsReachable)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(WalkPoint, sphereSize * 0.5f);
            }

            if (neighbors == null) return;

            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i] == null) continue;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(WalkPoint, neighbors[i].WalkPoint);
            }
        }

        public void InitializePerspectiveNodes()
        {
            if (perspectiveJoints == null || perspectiveJoints.Length == 0)
            {
                perspectiveJoints = new Vector2[4];
                perspectiveJoints[0] = Camera.main.WorldToScreenPoint(WalkPoint + transform.forward * 0.5f);
                perspectiveJoints[1] = Camera.main.WorldToScreenPoint(WalkPoint - transform.forward * 0.5f);
                perspectiveJoints[2] = Camera.main.WorldToScreenPoint(WalkPoint + transform.right * 0.5f);
                perspectiveJoints[3] = Camera.main.WorldToScreenPoint(WalkPoint - transform.right * 0.5f);
            }
        }

        public void AddAdjacentNeighbors()
        {
            RaycastHit hit;
            // Check 4 directions
            Vector3[] directions = new Vector3[] {
                transform.TransformDirection(Vector3.forward),
                transform.TransformDirection(Vector3.back),
                transform.TransformDirection(Vector3.left),
                transform.TransformDirection(Vector3.right),
            };

            for (int i = 0; i < directions.Length; i++)
            {
                if (Physics.Raycast(transform.position, directions[i], out hit))
                {
                    if (hit.distance > 1) continue;

                    NavNode target = hit.transform.gameObject.GetComponent<NavNode>();
                    if (target != null)
                    {
                        if (!IsNeighbor(target))
                        {
                            neighbors.Add(target);
                        }
                    }
                }
            } // for            
        }

        public void AddNeighbor(NavNode neighbor)
        {
            // TODO: add to RotativePlatform configuration if is a special case
            if (!neighbor.Equals(this) && !IsNeighbor(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        public void RemoveNeighbor(NavNode neighbor)
        {
            if (neighbor.Equals(this)) return;

            // Look for the neighbor
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i] == neighbor)
                {
                    neighbors.RemoveAt(i);
                    break;
                }
            }
        }

        public void ClearNeighbors()
        {
            neighbors.Clear();
        }

        public int ClearNullNeighbors()
        {
            int nullNeighbors = 0;
            for (int i = neighbors.Count -1 ; i >= 0; i--) 
            {
                if (neighbors[i] == null) 
                {
                    neighbors.RemoveAt(i);
                    nullNeighbors++;
                }
            }
            return nullNeighbors;
        }

        public bool IsNeighbor(NavNode walkable)
        {
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i] == walkable) return true;
            }

            return false;
        }

        /// <summary>
        /// Check if at least one joint is close to other joint
        /// </summary>
        /// <param name="jointsOtherNode"></param>
        /// <returns></returns>
        public bool AreJointsConnected(NavNode otherNode)
        {
            InitializePerspectiveNodes();

            Vector2[] jointsOtherNode = otherNode.perspectiveJoints;
            Vector2[] myJoints = perspectiveJoints;

            for (int i = 0; i < myJoints.Length; i++)
            {
                Vector2 jointToCheck = myJoints[i];
                for (int j = 0; j < jointsOtherNode.Length; j++)
                {
                    if (Vector2.Distance(jointToCheck, jointsOtherNode[j]) <= 0.1f)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}