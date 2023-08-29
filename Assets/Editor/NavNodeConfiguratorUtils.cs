using UnityEditor;
using UnityEngine;
using Monument.World;

namespace Monument.EditorUtils
{
    public class NavNodeConfiguratorUtils
    {
        // Setup every adjacent neighbor automatically
        public static void FindAdjacentNeighbors()
        {
            Debug.Log("Finding neighbors in scene...");
            NavNode[] sceneNodes = GameObject.FindObjectsOfType<NavNode>();

            for (int i = 0; i < sceneNodes.Length; i++)
            {
                var adjacentNodes = sceneNodes[i].GetAdjacentNodes();
                EditorUtility.SetDirty(sceneNodes[i]);

                for (int j = 0; j < adjacentNodes.Count; j++)
                {
                    sceneNodes[i].AddNeighbor(adjacentNodes[j]);
                }
            }
            Debug.Log("Neighbors setup successfully");
        }

        // Setup every adjacent neighbor automatically
        public static void FindNeighborsWithPerspective()
        {
            Debug.Log("Finding neighbors in scene...");
            NavNode[] sceneNodes = GameObject.FindObjectsOfType<NavNode>();

            for (int i = 0; i < sceneNodes.Length; i++)
            {
                //NavNode referenceNode = sceneNodes[i];
                sceneNodes[i].InitializePerspectiveNodes();

                for (int j = 0; j < sceneNodes.Length; j++)
                {
                    if (sceneNodes[j] == sceneNodes[i]) continue;

                    if (sceneNodes[j].AreJointsConnected(sceneNodes[i]))
                    {
                        sceneNodes[i].AddNeighbor(sceneNodes[j]);
                        sceneNodes[j].AddNeighbor(sceneNodes[i]);

                        EditorUtility.SetDirty(sceneNodes[i]);
                        EditorUtility.SetDirty(sceneNodes[j]);
                    }
                }
            }
            Debug.Log("Neighbors setup successfully");
        }

        // Remove every neighbor in scene
        public static void ClearNeighborsForEveryNode()
        {
            Debug.Log("Finding neighbors on scene...");
            NavNode[] sceneWalkables = GameObject.FindObjectsOfType<NavNode>();

            for (int i = 0; i < sceneWalkables.Length; i++)
            {
                sceneWalkables[i].ClearNeighbors();
                EditorUtility.SetDirty(sceneWalkables[i]);
            }
            Debug.Log("Neighbors cleaned successfully");
        }

        // Remove null neighbors in scene
        public static void ClearNullNeighbors()
        {
            int neighborsCleared = 0;
            Debug.Log("Finding neighbors on scene...");
            NavNode[] sceneWalkables = GameObject.FindObjectsOfType<NavNode>();

            for (int i = 0; i < sceneWalkables.Length; i++)
            {
                neighborsCleared += sceneWalkables[i].ClearNullNeighbors();
                EditorUtility.SetDirty(sceneWalkables[i]);
            }
            Debug.Log($"Neighbors cleaned successfully. Total null elements found: {neighborsCleared}");
        }
    }
}
