using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monument.World
{
    [System.Serializable]
    public class NavNode : MonoBehaviour
    {
        [System.Serializable]
        public struct NodeConfiguration
        {
            public bool isActive;
            public Vector3 offset;
        }

        [SerializeField] private List<NavNode> neighbors = new List<NavNode>();

        private Vector3[] directions = new Vector3[4] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

#if UNITY_EDITOR
        public bool ShowNeighborsFoldout = true;
        public bool ShowPossibleNeighborsFoldout = true;
        public bool DrawRaycastRays = false;
#endif
        public bool GlobalWalkpoint = false;

        public bool IsReachable { get; set; } = false; // Indicates if the node is reachable by the player

        public bool IsStairs = false; // Indicates the type of node

        public List<NavNode> Neighbors => neighbors;

        public RotativePlatform RotativePlatform { get; set; } = null;

        public bool HasMultipleConfiguration = false; // Has multiple walkpoints positions depending on current rotation

        public NodeConfiguration[] Configurations = new NodeConfiguration[4]; // 4 configurations (active and offset)

        private int currentConfiguration = 0;

        public Vector3 WalkPoint
        {
            get => GlobalWalkpoint ?
                transform.position + (Vector3.up * 0.5f + Configurations[currentConfiguration].offset) :
                transform.position + (transform.up * 0.5f + Configurations[currentConfiguration].offset);
        }

        // Private attributes
        private Vector2[] perspectiveJoints;

        // Gizmos parameters
        private const float sphereSize = 0.16f;
        private const float cubeJointsSize = 0.125f;

        public void InitializePerspectiveNodes()
        {
            perspectiveJoints = new Vector2[4];
            for (int i = 0; i < directions.Length; i++)
            {
                //perspectiveJoints[i] = Camera.main.WorldToScreenPoint(WalkPoint + directions[i] * 0.5f);
                perspectiveJoints[i] = Camera.main.WorldToScreenPoint(transform.position + directions[i] * 0.5f);
            }
        }

        public List<NavNode> GetPossibleNeighbors()
        {
            List<NavNode> adjacentNodes = NavNodeUtils.FindAdjacentNeighbors(this);
            List<NavNode> perspectiveNodes = NavNodeUtils.FindPerspectiveNodes(this);

            HashSet<NavNode> nodesSet = new HashSet<NavNode>(adjacentNodes);
            nodesSet.UnionWith(perspectiveNodes); //Join two lists without repeating elements
            nodesSet.ExceptWith(neighbors); // Remove nodes from the list that are already neighbors

            return new List<NavNode>(nodesSet);
        }

        public void AddNeighbor(NavNode node)
        {
            if (node != this && !neighbors.Contains(node))
            {
                neighbors.Add(node);
                node?.AddNeighbor(this);
            }
        }

        public void RemoveNeighbor(NavNode neighbor)
        {
            if (neighbor == this) return;

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

        public void RemoveNeighborAt(int index)
        {
            if (index >= neighbors.Count) return;

            neighbors.RemoveAt(index);
        }

        public void ClearNeighbors()
        {
            neighbors.Clear();
        }

        public void ApplyConfiguration(int configuration)
        {
            if (!HasMultipleConfiguration)
            {
                currentConfiguration = 0;
                return;
            }

            // If the configuration is active
            if (Configurations[configuration].isActive)
            {
                currentConfiguration = configuration;
            }
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

#if UNITY_EDITOR
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

            // Draw raycast rays
            if (DrawRaycastRays)
            {
                Gizmos.color = Color.red;
                if (IsStairs)
                {
                    Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.up - Vector3.right));
                    Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.down - Vector3.left));

                }
                else
                {
                    for (int i = 0; i < directions.Length; i++)
                    {
                        Gizmos.DrawRay(transform.position, directions[i]);
                    }
                }
            }
        }
#endif
    }
}