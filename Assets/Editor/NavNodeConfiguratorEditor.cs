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
    private Color[] rowColors = new Color[] { Color.white, Color.black }; // Alternating colors

    private NavNode focusedNode = null;

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
            DisplayNoSelectedNodesMessage();
            DisplayGeneralButtons();
            return;
        }

        DisplaySelectedNodes();
        DisplayGeneralButtons();
        ConfigEditorTool();
    }

    private void DisplayNoSelectedNodesMessage()
    {
        // Custom text area style with white background
        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.normal.background = Texture2D.whiteTexture;

        // Window Content
        EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical
        GUILayout.TextArea("Select GameObjects in the Hierarchy that have a NavNode component attached to visualize them here", textAreaStyle);
        GUILayout.TextArea($"There are {FindObjectsOfType<NavNode>().Length} NavNodes on scene", textAreaStyle);

        if (GUILayout.Button("Select every node in the scene"))
        {
            GameObject[] nodes = FindObjectsOfType<NavNode>().Select(navNode => navNode.gameObject).ToArray();
            Selection.objects = nodes;
        }

        EditorGUILayout.EndVertical();

        GUILayout.Space(5f);
    }

    private void DisplaySelectedNodes()
    {
        GUILayout.Label($"Selected {selectedNodes.Count} Nodes:", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // TODO: Explore selected RotativePlatforms Nodes

        // Display a row for each selected Node
        for (int i = 0; i < selectedNodes.Count; i++)
        {
            Color rowColor = rowColors[i % rowColors.Length];
            GUI.backgroundColor = rowColor;

            DisplayNode(selectedNodes[i]);
        }
        EditorGUILayout.EndScrollView();
    }

    private void DisplayNode(NavNode selectedNode)
    {
        // TODO: show adjacent nodes that are not neighbors to add them (create a button for every adjacent node)
        if (focusedNode == selectedNode)
        {
            GUI.backgroundColor = Color.green;
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical
        EditorGUILayout.BeginHorizontal(); // Node name and buttons on the same horizontal line

        // Flexible label for the GameObject's name
        GUIStyle nodeTitleStyle = new GUIStyle(EditorStyles.whiteLabel) { fontStyle = FontStyle.Bold };
        GUILayout.Label(selectedNode.name, nodeTitleStyle, GUILayout.ExpandWidth(true));

        // Reset color for buttons
        GUI.backgroundColor = Color.white;

        // Node buttons creation        
        CreateNodeButton("Focus", ref selectedNode, () => FocusOnNode(selectedNode));
        CreateNodeButton("Rename", ref selectedNode, null);
        CreateNodeButton("Setup neighbors", ref selectedNode, null);
        CreateNodeButton("Clear", ref selectedNode, selectedNode.ClearNeighbors);

        EditorGUILayout.EndHorizontal();

        // Neighbors info
        DisplayNeighborsInfo(ref selectedNode);
        EditorGUILayout.EndVertical();
    }

    private void CreateNodeButton(string buttonString, ref NavNode selectedNode, System.Action buttonAction)
    {
        // Button with size adjusted to fit the text
        if (GUILayout.Button(buttonString, GUILayout.Width(EditorStyles.miniButton.CalcSize(new GUIContent(buttonString)).x)))
        {
            if (buttonAction == null) return;

            Undo.RecordObject(selectedNode, buttonAction.Method.Name); // Record the object for undo
            buttonAction();
            EditorUtility.SetDirty(selectedNode);
            Debug.Log($"Clicked {buttonAction.Method.Name} on {selectedNode.name}");
        }
    }

    private void DisplayNeighborsInfo(ref NavNode selectedNode)
    {
        if (selectedNode.Neighbors.Count == 0)
        {
            EditorGUILayout.LabelField($"{selectedNode.Neighbors.Count} Neighbors:");
            return;
        }

        selectedNode.ShowNeighborsFoldout = EditorGUILayout.Foldout(selectedNode.ShowNeighborsFoldout, $"{selectedNode.Neighbors.Count} Neighbors:");

        if (selectedNode.ShowNeighborsFoldout)
        {
            for (int j = 0; j < selectedNode.Neighbors.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                selectedNode.Neighbors[j] = (NavNode)EditorGUILayout.ObjectField(selectedNode.Neighbors[j], typeof(NavNode), allowSceneObjects: true, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    // Display primary buttons with functions that impact every node in the scene
    private void DisplayGeneralButtons()
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
        if (GUILayout.Button("Setup every neighbor in the scene"))
        {

        }
        if (GUILayout.Button("Rename every node in the scene"))
        {

        }
        if (GUILayout.Button("Clear neighbors for all nodes in the scene"))
        {

        }
        EditorGUILayout.EndVertical();
    }

    private void FocusOnNode(NavNode nodeToFocus)
    {
        // Assign focusedNode value
        focusedNode = nodeToFocus; //selectedNodes.FindIndex(0, selectedNodes.Count, node => node == nodeToFocus);        

        // Create bounds around object to focus
        Bounds bounds = new Bounds(nodeToFocus.transform.position, nodeToFocus.transform.localScale * 3f);
        SceneView.lastActiveSceneView.Frame(bounds, false);
    }

    // TODO: serialize configuration and save it locally
    private void ConfigEditorTool()
    {
        GUILayout.Space(5f);

        // Panel color
        GUI.backgroundColor = Color.white;

        // Custom text area style with white background
        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.normal.background = Texture2D.whiteTexture;

        // Title
        GUILayout.Label("Tool appearance", new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 });

        EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical        

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Primary color");
        rowColors[0] = EditorGUILayout.ColorField(rowColors[0]);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Secondary color");
        rowColors[1] = EditorGUILayout.ColorField(rowColors[1]);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }
}