using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Monument.World;

public class UtilsMenuItems
{
    // Add a menu item named "Do Something" to MyMenu in the menu bar.
    // Setup every adjacent neighbor automatically
    [MenuItem("Walkables/Find Neighbors in the scene")]
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
}
