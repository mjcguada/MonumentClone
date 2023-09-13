using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Monument.World;
using System.Linq;
using Monument.EditorUtils;
using System;
using static Unity.VisualScripting.Member;

public class NavNodeConfigurator : EditorWindow
{
    // Node selection scroll view
    private List<NavNode> selectedNodes = new List<NavNode>(); // Nodes selected in hierarchy
    private Vector2 scrollPosition;
    private Color[] rowColors = new Color[] { Color.white, Color.black }; // Alternating colors

    // Focused node features
    private NavNode focusedNode = null;
    private List<NavNode> possibleNeighbors = new List<NavNode>();

    // Parameters for configuring Scene View
    // TODO:
    private bool showNodeLabels = false;
    private bool showNeighbors = true;
    private bool showPossibleNeighbors = true;

    private int selectedMode = 0; // 0 = selection mode, 1 = scene mode

    private string[] editorModes = new string[] { "Show Selection", "Show All" };
    private List<NavNode> nodesToShow = new List<NavNode>();
    private List<NavNode> everyNode = new List<NavNode>();

    // GUIStyles
    private const int titleFontSize = 12;
    private GUIStyle labelGUIStyle = new GUIStyle();
    private Color selectedNodeColor = Color.red;
    private Color neighborsColor = Color.green;
    private Color possibleNeighborsColor = Color.cyan;

    [MenuItem("Tools/Nodes Configurator")]
    public static void ShowWindow()
    {
        GetWindow<NavNodeConfigurator>("Nodes Configurator");
    }

    private void Awake()
    {
        labelGUIStyle.fontSize = 10;
        labelGUIStyle.normal = new GUIStyleState();
        labelGUIStyle.normal.textColor = Color.black;
        labelGUIStyle.normal.background = Texture2D.whiteTexture;

        everyNode = FindObjectsOfType<NavNode>().ToList();
    }

    private void OnEnable()
    {
        // Register the selection changed event
        Selection.selectionChanged += UpdateSelectedGameObjects;
        UpdateSelectedGameObjects();

        // Update everyNode list collection everytime a gameObject is created/destroyed
        EditorApplication.hierarchyChanged += OnHierarchyChanged;

        // Subscribe to the duringSceneGui event to handle drawing in the SceneView
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        // Unsubscribe the selection changed event when closing the window
        Selection.selectionChanged -= UpdateSelectedGameObjects;

        // Unsubscribe from the hierarchyChanged event when disabling the window
        EditorApplication.hierarchyChanged -= OnHierarchyChanged;

        // Unsubscribe from the duringSceneGui event when disabling the window
        SceneView.duringSceneGui -= OnSceneGUI;
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

            if (selectedNode == null || !selectedGameObjects.Contains(selectedNode.gameObject))
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

        // If the focused node is no longer selected, we set it to null
        if (!selectedNodes.Contains(focusedNode))
        {
            focusedNode = null;
            possibleNeighbors.Clear();
        }

        Repaint(); // Force window to repaint
    }

    // This is what we're showing in the Scene View
    private void OnSceneGUI(SceneView sceneView)
    {
        if (focusedNode != null)
        {
            // Possible Neighbors drawing
            Handles.color = possibleNeighborsColor;
            for (int i = 0; i < possibleNeighbors.Count; i++)
            {
                Handles.DrawWireCube(possibleNeighbors[i].transform.position, possibleNeighbors[i].transform.localScale);
            }

            // TODO: Neighbors drawing

            // Focused Node drawing
            Handles.color = selectedNodeColor;
            Handles.DrawWireCube(focusedNode.transform.position, focusedNode.transform.localScale);

            // Draw arrows of position handle
            EditorGUI.BeginChangeCheck(); // Begin change check

            // Use PositionHandle to draw and manipulate the object's position
            Vector3 newPosition = Handles.PositionHandle(focusedNode.transform.position, focusedNode.transform.rotation);

            if (EditorGUI.EndChangeCheck()) // Check if a change has been made
            {
                Undo.RecordObject(focusedNode.transform, "Move Node"); // Allow for undoing the movement
                focusedNode.transform.position = newPosition; // Update the object's position
            }

            Handles.Label(focusedNode.transform.position, $"Focused Node ({focusedNode.name})", labelGUIStyle);

            // Labels drawing at the end
            for (int i = 0; i < possibleNeighbors.Count; i++)
            {
                Handles.Label(possibleNeighbors[i].transform.position, possibleNeighbors[i].name, labelGUIStyle);
            }
        }

        // Selected nodes names drawing
        if (showNodeLabels)
        {
            for (int i = 0; i < selectedNodes.Count; i++)
            {
                Handles.Label(selectedNodes[i].transform.position, selectedNodes[i].name, labelGUIStyle);
            }
        }
    }

    // This is what we're showing in the Window tool
    private void OnGUI()
    {
        GUILayout.Space(5f);

        DisplayEditorModeSelectionToolbar();

        EditorGUILayout.BeginHorizontal();

        // Neighbors panel
        DisplayNeighborsActions();

        // Possible Neighbors panel
        DisplayPossibleNeighborsActions();

        EditorGUILayout.EndHorizontal();

        DisplayGeneralButtons();

        DisplayNodes();
    }

    // This function is called every time the project changes (destroying/creating/renaming an object)
    private void OnHierarchyChanged()
    {
        everyNode = FindObjectsOfType<NavNode>().ToList();
    }

    private void UpdateNodeCollection()
    {
        switch (selectedMode)
        {
            case 0:
                nodesToShow = selectedNodes;
                break;
            case 1:
                // TODO: Order by name
                nodesToShow = everyNode;
                break;
            default:
                break;
        }
    }

    private void DisplayEditorModeSelectionToolbar()
    {
        // Panel color
        GUI.backgroundColor = Color.white;
        EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical

        // Custom text area style with white background
        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.normal.background = Texture2D.whiteTexture;

        // Title
        GUILayout.Label("Node Visualization", new GUIStyle(EditorStyles.boldLabel) { fontSize = titleFontSize });

        // Description
        GUILayout.Label("Choose between visualizing nodes selected in the hierarchy or showing every node in the scene", textAreaStyle);

        GUILayout.BeginVertical();
        selectedMode = GUILayout.Toolbar(selectedMode, editorModes);
        GUILayout.EndVertical();
        UpdateNodeCollection();

        EditorGUILayout.EndVertical();
    }

    private void DisplayNeighborsActions()
    {
        // Panel color
        GUI.backgroundColor = Color.cyan;
        EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical

        // Custom text area style with white background
        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.normal.background = Texture2D.whiteTexture;

        // Title
        GUILayout.Label("Neighbors Visualization", new GUIStyle(EditorStyles.boldLabel) { fontSize = titleFontSize });

        showNeighbors = EditorGUILayout.Toggle("Show Neighbors", showNeighbors);

        if (GUILayout.Button("Expand all"))
        {
            ExpandNeighborsOnInspector(true);
        }
        if (GUILayout.Button("Collapse all"))
        {
            ExpandNeighborsOnInspector(false);
        }
        EditorGUILayout.EndVertical(); // - Vertical
    }

    private void ExpandNeighborsOnInspector(bool expand) 
    { 
        for (int i = 0; i < everyNode.Count; i++) 
        {
            everyNode[i].ShowNeighborsFoldout = expand;
        }
    }

    private void DisplayPossibleNeighborsActions()
    {
        // Panel color
        GUI.backgroundColor = new Color(0.9f, 0.4f, 0); // Orange color
        EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical

        // Custom text area style with white background
        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.normal.background = Texture2D.whiteTexture;

        // Title
        GUILayout.Label("Possible Neighbors", new GUIStyle(EditorStyles.boldLabel) { fontSize = titleFontSize });

        showPossibleNeighbors = EditorGUILayout.Toggle("Show Possible Neighbors", showPossibleNeighbors);

        if (GUILayout.Button("Expand all"))
        {
            ExpandPossibleNeighborsOnInspector(true);
        }
        if (GUILayout.Button("Collapse all"))
        {
            ExpandPossibleNeighborsOnInspector(false);
        }
        EditorGUILayout.EndVertical(); // - Vertical
    }

    private void ExpandPossibleNeighborsOnInspector(bool expand)
    {
        for (int i = 0; i < everyNode.Count; i++)
        {
            everyNode[i].ShowPossibleNeighborsFoldout = expand;
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
        GUILayout.Label("General Actions", new GUIStyle(EditorStyles.boldLabel) { fontSize = titleFontSize });
        GUILayout.Label("(Affect every node in scene)");

        // Description
        //GUILayout.Label("Actions that affect every Node in the scene");

        // Primary buttons
        if (GUILayout.Button("Setup every neighbor"))
        {
            NavNodeConfiguratorUtils.FindNeighborsWithPerspective();
        }
        if (GUILayout.Button("Rename every node"))
        {

        }
        if (GUILayout.Button("Clear every neighbor"))
        {
            NavNodeConfiguratorUtils.ClearNeighborsForEveryNode();
        }
        EditorGUILayout.EndVertical();
    }

    private void DisplayNoSelectedNodesMessage()
    {
        GUI.backgroundColor = Color.white;

        // Custom text area style with white background
        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.normal.background = Texture2D.whiteTexture;

        // Window Content
        GUILayout.Label($"Select GameObjects in the Hierarchy that have a NavNode component attached to visualize them here\n" +
            $"There are {FindObjectsOfType<NavNode>().Length} Navigation Nodes in the scene", textAreaStyle);

        GUILayout.Space(5f);
    }

    private void DisplayNodes()
    {
        if (nodesToShow.Count == 0)
        {
            DisplayNoSelectedNodesMessage();
            return;
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.Label($"Selected {nodesToShow.Count} Nodes:", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // TODO: Explore selected RotativePlatforms Nodes

        // Display a row for each selected Node
        for (int i = 0; i < nodesToShow.Count; i++)
        {
            Color rowColor = rowColors[i % rowColors.Length];
            GUI.backgroundColor = rowColor;

            DisplayNode(nodesToShow[i]);
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DisplayNode(NavNode nodeToShow)
    {
        if (nodeToShow == null) { return; }

        // TODO: show adjacent nodes that are not neighbors to add them (create a button for every adjacent node)
        if (focusedNode == nodeToShow)
        {
            GUI.backgroundColor = Color.green;
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical
        EditorGUILayout.BeginHorizontal(); // Node name and buttons on the same horizontal line

        // Flexible label for the GameObject's name
        GUIStyle nodeTitleStyle = new GUIStyle(EditorStyles.whiteLabel) { fontStyle = FontStyle.Bold };
        GUILayout.Label(nodeToShow.name, nodeTitleStyle, GUILayout.ExpandWidth(true));

        // Reset color for buttons
        GUI.backgroundColor = Color.white;

        // Node buttons creation        
        CreateNodeButton("Focus", ref nodeToShow, () => FocusOnNode(nodeToShow), setDirty: false);
        //CreateNodeButton("Rename", ref selectedNode, null);
        //CreateNodeButton("Setup neighbors", ref selectedNode, null);
        CreateNodeButton("Clear neighbors", ref nodeToShow, nodeToShow.ClearNeighbors);

        EditorGUILayout.EndHorizontal();

        // Neighbors info
        if (showNeighbors) DisplayNeighborsInfo(ref nodeToShow);

        // Possible neighborsInfo
        if (showPossibleNeighbors) DisplayPossibleNeighbors(ref nodeToShow);

        EditorGUILayout.EndVertical();
    }

    private void CreateNodeButton(string buttonString, ref NavNode selectedNode, System.Action buttonAction, bool setDirty = true)
    {
        // Button with size adjusted to fit the text
        if (GUILayout.Button(buttonString, GUILayout.Width(EditorStyles.miniButton.CalcSize(new GUIContent(buttonString)).x)))
        {
            if (buttonAction == null) return;

            Undo.RecordObject(selectedNode, buttonAction.Method.Name); // Record the object for undo
            buttonAction();
            if(setDirty) EditorUtility.SetDirty(selectedNode);
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

        EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical

        selectedNode.ShowNeighborsFoldout = EditorGUILayout.Foldout(selectedNode.ShowNeighborsFoldout, $"{selectedNode.Neighbors.Count} Neighbors:");

        EditorGUI.indentLevel++;
        if (selectedNode.ShowNeighborsFoldout)
        {
            for (int i = 0; i < selectedNode.Neighbors.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(selectedNode.Neighbors[i], typeof(NavNode), allowSceneObjects: true);

                // Remove neighbor button
                if (GUILayout.Button("Remove node", GUILayout.Width(150)))
                {

                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    private void DisplayPossibleNeighbors(ref NavNode selectedNode)
    {
        GUI.backgroundColor = new Color(0.9f, 0.4f, 0); // Orange color

        List<NavNode> possibleNeighbors = selectedNode.GetAdjacentNodes();
        // TODO: remove nodes that are already neighbors

        if (possibleNeighbors.Count == 0)
        {
            //EditorGUILayout.LabelField($"{possibleNeighbors.Count} Possible Neighbors:");
            return;
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical

        selectedNode.ShowPossibleNeighborsFoldout = EditorGUILayout.Foldout(selectedNode.ShowPossibleNeighborsFoldout, $"{possibleNeighbors.Count} Possible Neighbors:");

        EditorGUI.indentLevel++;
        if (selectedNode.ShowPossibleNeighborsFoldout)
        {
            for (int i = 0; i < possibleNeighbors.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(possibleNeighbors[i], typeof(NavNode), allowSceneObjects: true);

                // Add neighbor button
                if (GUILayout.Button("Add node", GUILayout.Width(150)))
                {

                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }





    private void FocusOnNode(NavNode nodeToFocus)
    {
        // Assign focusedNode value
        focusedNode = nodeToFocus; //selectedNodes.FindIndex(0, selectedNodes.Count, node => node == nodeToFocus);

        // TODO: change function
        possibleNeighbors = focusedNode.GetAdjacentNodes();

        // Create bounds around object to focus
        Bounds bounds = new Bounds(nodeToFocus.transform.position, nodeToFocus.transform.localScale * 3f);
        SceneView.lastActiveSceneView.Frame(bounds, false);
    }
}