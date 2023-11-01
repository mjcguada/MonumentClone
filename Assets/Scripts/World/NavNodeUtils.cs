using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace Monument.World
{
    public class NavNodeUtils
    {
        private static Vector3[] directions = new Vector3[4] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

        // Setup every possible neighbor automatically (adjacent and perspective)
        public static void SetUpNeighborsAutomatically()
        {
            Debug.Log("Finding neighbors in scene...");
            NavNode[] sceneNodes = GameObject.FindObjectsOfType<NavNode>();

            int neighborCount = 0;

            for (int i = 0; i < sceneNodes.Length; i++)
            {
                List<NavNode> perspectiveNodes = FindPerspectiveNodes(sceneNodes[i], sceneNodes);
                List<NavNode> adjacentNodes = FindAdjacentNeighbors(sceneNodes[i]);

                HashSet<NavNode> nodesSet = new HashSet<NavNode>(adjacentNodes);
                nodesSet.UnionWith(perspectiveNodes); //Join two lists without repeating elements

                List<NavNode> neighbors = new List<NavNode>(nodesSet);
                neighborCount += neighbors.Count;

                // Add nodes
                for (int j = 0; j < neighbors.Count; j++)
                {
                    sceneNodes[i].AddNeighbor(neighbors[j]);
                }
            }

            Debug.Log($"{neighborCount} Neighbors setup successfully");
        }

        public static List<NavNode> FindPerspectiveNodes(NavNode nodeToExamine, NavNode[] sceneCollection = null)
        {
            List<NavNode> perspectiveNodes = new List<NavNode>();

            if (nodeToExamine.IsStairs) return perspectiveNodes;

            // Find every NavNode object in the scene
            if (sceneCollection == null) sceneCollection = GameObject.FindObjectsOfType<NavNode>();

            nodeToExamine.InitializePerspectiveNodes();

            for (int i = 0; i < sceneCollection.Length; i++)
            {
                if (nodeToExamine == sceneCollection[i]) continue;

                if (sceneCollection[i].AreJointsConnected(nodeToExamine))
                {
                    perspectiveNodes.Add(sceneCollection[i]);
                }
            }
            return perspectiveNodes;
        }

        public static List<NavNode> FindAdjacentNeighbors(NavNode nodeToExamine)
        {
            List<NavNode> adjacentNodes = new List<NavNode>();

            if (nodeToExamine.IsStairs)
            {
                RaycastHit hit;
                // up and forward (we use up - right because the stairs mesh is rotated)
                if (Physics.Raycast(nodeToExamine.transform.position, nodeToExamine.transform.TransformDirection(Vector3.up - Vector3.right).normalized, out hit))
                {
                    if (hit.distance <= 1) TryToAddNode(hit);
                }

                // down and backwards (we use down - left because the stairs mesh is rotated)
                if (Physics.Raycast(nodeToExamine.transform.position, nodeToExamine.transform.TransformDirection(Vector3.down - Vector3.left).normalized, out hit))
                {
                    if (hit.distance <= 1) TryToAddNode(hit);
                }
            }
            else
            {
                // Check 4 directions
                for (int i = 0; i < directions.Length; i++)
                {
                    if (Physics.Raycast(nodeToExamine.transform.position, directions[i], out RaycastHit hit))
                    {
                        if (hit.distance <= 1) TryToAddNode(hit);
                    }
                }
            }
            return adjacentNodes;

            void TryToAddNode(RaycastHit hit)
            {
                NavNode target = hit.transform.gameObject.GetComponent<NavNode>();

                if (TargetIsSuitable(target))
                {
                    adjacentNodes.Add(target);
                }

                if (target == null)
                {
                    // Try to find the component in the father
                    target = hit.transform.gameObject.GetComponentInParent<NavNode>(); // stairs
                    if (TargetIsSuitable(target)) adjacentNodes.Add(target);
                }
            }

            bool TargetIsSuitable(NavNode target)
            {
                if (target == null) return false;
                if (target == nodeToExamine) return false;
                if (adjacentNodes.Contains(target)) return false;

                return true;
            }
        }

#if UNITY_EDITOR
        // Remove every neighbor in scene
        public static void ClearNeighborsForEveryNode()
        {
            Debug.Log("Finding neighbors on scene...");
            NavNode[] sceneWalkables = GameObject.FindObjectsOfType<NavNode>();

            if (sceneWalkables.Length == 0)
            {
                Debug.Log("There's no NavNodes on scene to clear");
                return;
            }

            // Get the current Undo group
            int undoGroup = Undo.GetCurrentGroup();

            for (int i = 0; i < sceneWalkables.Length; i++)
            {
                // Record changes for this node
                Undo.RecordObject(sceneWalkables[i], "Clear Neighbors");
                sceneWalkables[i].ClearNeighbors();
                EditorUtility.SetDirty(sceneWalkables[i]);
            }

            // Collapse Undo operations into a single step
            Undo.CollapseUndoOperations(undoGroup);
            Debug.Log("Neighbors cleaned successfully");
        }

        public static void ClearNULLNeighborsForEveryNode()
        {
            Debug.Log("Finding neighbors on scene...");
            NavNode[] sceneWalkables = GameObject.FindObjectsOfType<NavNode>();

            if (sceneWalkables.Length == 0)
            {
                Debug.Log("There's no NavNodes on scene to clear");
                return;
            }

            // Get the current Undo group
            int undoGroup = Undo.GetCurrentGroup();

            for (int i = 0; i < sceneWalkables.Length; i++)
            {
                // Record changes for this node
                Undo.RecordObject(sceneWalkables[i], "Clear Neighbors");
                sceneWalkables[i].ClearNullNeighbors();
                EditorUtility.SetDirty(sceneWalkables[i]);
            }

            // Collapse Undo operations into a single step
            Undo.CollapseUndoOperations(undoGroup);
            Debug.Log("Null neighbors cleaned successfully");
        }
#endif
    }
}