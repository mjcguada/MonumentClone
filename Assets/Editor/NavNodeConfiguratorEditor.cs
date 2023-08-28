using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Monument.World;
using System.Linq;

public class NavNodeConfiguratorEditor : EditorWindow
{
    private List<NavNode> selectedNodes = new List<NavNode>();
    private Vector2 scrollPosition;
    private Color[] rowColors = new Color[] { Color.white, Color.red }; // Alternating colors

    [MenuItem("Window/Nodes Configurator")]
    public static void ShowWindow()
    {
        GetWindow<NavNodeConfiguratorEditor>("Nodes Configurator");
    }

    private void OnEnable()
    {
        // Register the selection changed event
        Selection.selectionChanged += UpdateSelectedGameObjects;
        UpdateSelectedGameObjects();
    }

    private void OnDisable()
    {
        // Deregister the selection changed event when closing the window
        Selection.selectionChanged -= UpdateSelectedGameObjects;
    }

    private void UpdateSelectedGameObjects()
    {
        // Update the list with new selected Nodes
        GameObject[] selectedGameObjects = Selection.gameObjects;

        // We want them ordered by selection order, so:
        // 1. Check if we have to remove something from the list (because it's not selected)
        // We iterate over the list backwards to avoid index errors
        for (int i = selectedNodes.Count - 1; i >= 0; i--)
        {
            NavNode selectedNode = selectedNodes[i];

            if (!selectedGameObjects.Contains(selectedNode.gameObject))
            {
                selectedNodes.RemoveAt(i);
            }
        }

        // 2. We add new Nodes to the list
        for (int i = 0; i < selectedGameObjects.Length; i++)
        {
            NavNode selectedNode = selectedGameObjects[i].GetComponent<NavNode>();

            if (selectedNode == null) continue;

            if (!selectedNodes.Contains(selectedNode))
            {
                selectedNodes.Add(selectedNode);
            }
        }

        Repaint(); // Force window to repaint
    }

    private void OnGUI()
    {
        GUILayout.Space(5f);

        if (selectedNodes.Count == 0)
        {
            // Custom text area style with white background
            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
            textAreaStyle.normal.background = Texture2D.whiteTexture;

            NavNode[] nodesInScene = FindObjectsOfType<NavNode>();

            // Window Content
            EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical
            GUILayout.TextArea("Select GameObjects in the Hierarchy that have a NavNode component attached to visualize them here", textAreaStyle);
            GUILayout.TextArea($"There are {nodesInScene.Length} NavNodes on scene", textAreaStyle);
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(5f);

            ShowGeneralButtons();
            return;
        }

        GUILayout.Label($"Selected {selectedNodes.Count} Nodes:", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // TODO: Explore selected RotativePlatforms Nodes

        // Display a row for each selected Node
        for (int i = 0; i < selectedNodes.Count; i++)
        {
            Color rowColor = rowColors[i % rowColors.Length];
            GUI.backgroundColor = rowColor;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical
            EditorGUILayout.BeginHorizontal();
            NavNode selectedNode = selectedNodes[i];

            // Flexible label for the GameObject's name
            GUIStyle selectedNodeStyle = new GUIStyle(EditorStyles.whiteLabel) { fontStyle = FontStyle.Bold };
            GUILayout.Label(selectedNode.name, selectedNodeStyle, GUILayout.ExpandWidth(true));

            // Fixed-size button
            if (GUILayout.Button("Rename", GUILayout.Width(80)))
            {
                // TODO: SetDirty()
                Debug.Log("Rename button clicked for Node: " + selectedNode.name);
            }

            // Fixed-size button
            if (GUILayout.Button("Setup neighbors", GUILayout.Width(120)))
            {
                // TODO: SetDirty()
                Debug.Log("Setup neighbors button clicked for Node: " + selectedNode.name);
            }

            // Fixed-size button
            if (GUILayout.Button("Clear", GUILayout.Width(60)))
            {
                // TODO: SetDirty()
                Debug.Log("Clear neighbors button clicked for Node: " + selectedNode.name);
            }

            EditorGUILayout.EndHorizontal();

            // Neighbors info
            EditorGUILayout.BeginHorizontal();

            // Special GUIStyle with smaller font size
            GUIStyle neighborsInfoSizeStyle = new GUIStyle(EditorStyles.whiteLabel) { fontSize = 10 };

            GUILayout.Label("Number of neighbors: " + selectedNode.Neighbors.Count.ToString(), neighborsInfoSizeStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        ShowGeneralButtons();
    }

    // Display primary buttons with functions that impact every node in the scene
    private void ShowGeneralButtons()
    {
        // Panel color
        GUI.backgroundColor = new Color(0.4f, 0.9f, 0.6f);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical

        // Custom text area style with white background
        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.normal.background = Texture2D.whiteTexture;

        // Title
        GUILayout.Label("Primary Actions", new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 });

        // Description
        GUILayout.Label("The following actions affect every NavNode on scene");

        // Primary buttons
        if (GUILayout.Button("Setup every node in the scene"))
        {

        }
        if (GUILayout.Button("Clear Neighbors for all nodes in scene"))
        {

        }
        if (GUILayout.Button("Setup every node on scene"))
        {

        }
        EditorGUILayout.EndVertical();
    }
}
