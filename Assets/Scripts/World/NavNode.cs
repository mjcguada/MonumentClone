using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monument.World
{
    [System.Serializable]
    public class NavNode : MonoBehaviour
    {
        [SerializeField] private float _offset = 1f;
        [SerializeField] private List<NavNode> neighbors = new List<NavNode>();

        private Vector3[] directions = new Vector3[4] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

#if UNITY_EDITOR
        public bool ShowNeighborsFoldout { get; set; } = false;
        public bool ShowPossibleNeighborsFoldout { get; set; } = false;
#endif
        public bool GlobalWalkpoint { get; set; } = false;

        public bool IsReachable { get; set; } = false; // Indicates if the node is reachable by the player

        public List<NavNode> Neighbors => neighbors;

        public RotativePlatform RotativePlatform { get; set; } = null;

        public bool HasMultipleConfiguration { get; set; } = false; // Has multiple walkpoints positions depending on current rotation

        public (bool, Vector3)[] Configurations { get; set; } = new (bool, Vector3)[4];

        private int currentConfiguration = 0;

        public Vector3 WalkPoint
        {
            get => GlobalWalkpoint ?
                transform.position + (Vector3.up * 0.5f * _offset) :
                transform.position + (transform.up * 0.5f * _offset);
        }

        // Private attributes
        private Vector2[] perspectiveJoints;

        // Gizmos parameters
        private const float sphereSize = 0.16f;
        private const float cubeJointsSize = 0.125f;        

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
            // TODO: add to RotativePlatform configuration if is a special case
            if (node != this && !neighbors.Contains(node))
            {
                neighbors.Add(node);
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

        public void ClearNeighbors()
        {
            neighbors.Clear();
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
    }
}