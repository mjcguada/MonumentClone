using UnityEditor;
using UnityEngine;
using Monument.World;

namespace Monument.EditorUtils
{
    public class UtilsMenuItems
    {
        // Setup every adjacent neighbor automatically
        [MenuItem("Walkables/Find Adjacent neighbors on scene")]
        public static void FindNeighbors()
        {
            Debug.Log("Finding neighbors in scene...");
            NavNode[] sceneWalkables = GameObject.FindObjectsOfType<NavNode>();

            for (int i = 0; i < sceneWalkables.Length; i++)
            {
                sceneWalkables[i].AddAdjacentNeighbors();
                EditorUtility.SetDirty(sceneWalkables[i]);
            }
            Debug.Log("Neighbors setup successfully");
        }

        // Setup every adjacent neighbor automatically
        [MenuItem("Walkables/Find neighbors with perspective")]
        public static void FindNeighborsWithPerspective()
        {
            Debug.Log("Finding neighbors in scene...");
            NavNode[] sceneNodes = GameObject.FindObjectsOfType<NavNode>();

            for (int i = 0; i < sceneNodes.Length; i++)
            {
                NavNode referenceNode = sceneNodes[i];
                referenceNode.InitializePerspectiveNodes();

                for (int j = 0; j < sceneNodes.Length; j++)
                {
                    if (sceneNodes[j].Equals(sceneNodes[i])) continue;

                    if (sceneNodes[j].AreJointsConnected(referenceNode))
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
        [MenuItem("Walkables/Clear neighbors on scene")]
        public static void ClearNeighbors()
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

        // Remove every neighbor in scene
        [MenuItem("Walkables/Clear NULL neighbors on scene")]
        public static void ClearNullNeighbors()
        {
            Debug.Log("Finding neighbors on scene...");
            NavNode[] sceneWalkables = GameObject.FindObjectsOfType<NavNode>();

            for (int i = 0; i < sceneWalkables.Length; i++)
            {
                sceneWalkables[i].ClearNullNeighbors();
                EditorUtility.SetDirty(sceneWalkables[i]);
            }
            Debug.Log("Neighbors cleaned successfully");
        }
    }
}
