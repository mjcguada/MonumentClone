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

        private Vector3[] directions = new Vector3[4] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

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
            for (int i = 0; i < directions.Length; i++)
            {
                Gizmos.DrawLine(WalkPoint, WalkPoint + directions[i] * 0.5f);
            }

            Gizmos.color = Color.blue;
            // Draw 4 cube joints
            for (int i = 0; i < directions.Length; i++)
            {
                Gizmos.DrawCube(WalkPoint + directions[i] * 0.5f, Vector3.one * cubeJointsSize);
            }

            // Reachable from player's position
            if (IsReachable)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(WalkPoint, sphereSize * 0.5f);
            }

            if (neighbors == null) return;

            // Draws join lines between nodes
            Gizmos.color = Color.green;
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i] == null) continue;

                Gizmos.DrawLine(WalkPoint, neighbors[i].WalkPoint);
            }
        }

        public void InitializePerspectiveNodes()
        {
            if (perspectiveJoints == null || perspectiveJoints.Length == 0)
            {
                perspectiveJoints = new Vector2[4];
                for (int i = 0; i < directions.Length; i++)
                {
                    perspectiveJoints[i] = Camera.main.WorldToScreenPoint(WalkPoint + directions[i] * 0.5f);
                }
            }
        }

        public List<NavNode> GetAdjacentNodes()
        {
            List<NavNode> adjacentNodes = new List<NavNode>();
            RaycastHit hit;

            // Check 4 directions
            for (int i = 0; i < directions.Length; i++)
            {
                if (Physics.Raycast(transform.position, directions[i], out hit))
                {
                    if (hit.distance > 1) continue;

                    NavNode target = hit.transform.gameObject.GetComponent<NavNode>();

                    if (target != null && target != this)
                    {
                        adjacentNodes.Add(target);
                    }
                }
            } // for
            return adjacentNodes;
        }

        public void AddNeighbor(NavNode node)
        {
            // TODO: add to RotativePlatform configuration if is a special case
            if (node != this && !neighbors.Contains(node))
            {
                neighbors.Add(node);
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
            for (int i = neighbors.Count - 1; i >= 0; i--)
            {
                if (neighbors[i] == null)
                {
                    neighbors.RemoveAt(i);
                    nullNeighbors++;
                }
            }
            return nullNeighbors;
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