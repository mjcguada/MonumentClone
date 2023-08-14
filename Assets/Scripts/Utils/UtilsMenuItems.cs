using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Monument.World;

public class UtilsMenuItems
{
    // Setup every adjacent neighbor automatically
    [MenuItem("Walkables/Find Adjacent neighbors on scene")]
    public static void FindNeighbors()
    {
        Debug.Log("Finding neighbors in scene...");
        Walkable[] sceneWalkables = GameObject.FindObjectsOfType<Walkable>();

        for (int i = 0; i < sceneWalkables.Length; i++)
        {
            sceneWalkables[i].AddAdjacentNeighbors();
            EditorUtility.SetDirty(sceneWalkables[i]);
        }
        Debug.Log("Neighbors setup successfully");
    }

    
    // Remove every neighbor in scene
    [MenuItem("Walkables/Clear neighbors on scene")]
    public static void ClearNeighbors()
    {
        Debug.Log("Finding neighbors on scene...");
        Walkable[] sceneWalkables = GameObject.FindObjectsOfType<Walkable>();

        for (int i = 0; i < sceneWalkables.Length; i++)
        {
            sceneWalkables[i].ClearNeighbors();
            EditorUtility.SetDirty(sceneWalkables[i]);
        }
        Debug.Log("Neighbors cleaned successfully");
    }
}
