using Monument.World;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavNode))]
public class NavNodeEditor : Editor
{
    //private NavNode navNode;

    //private void OnSceneGUI()
    //{
    //    if(navNode == null) navNode = target as NavNode;

    //    Handles.color = Color.red;
    //    Handles.DrawWireCube(navNode.transform.position, navNode.transform.localScale);
    //    Handles.Label(navNode.WalkPoint, "Focused Node");
    //}
}
