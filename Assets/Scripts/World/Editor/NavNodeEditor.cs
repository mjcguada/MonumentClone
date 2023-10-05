using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Monument.World
{
    [CustomEditor(typeof(NavNode))]
    public class NavNodeEditor : Editor
    {
        private NavNode node;

        private void OnEnable()
        {
            node = target as NavNode;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck(); // Start change check

            node.GlobalWalkpoint = EditorGUILayout.Toggle("Global Walkpoint", node.GlobalWalkpoint);

            node.IsStairs = EditorGUILayout.Toggle("Is Stairs", node.IsStairs);
            
            node.DrawRaycastRays = EditorGUILayout.Toggle("Draw Raycast Rays", node.DrawRaycastRays);

            DisplayNeighborsFoldout();

            DisplayPossibleNeighborsFoldout();

            // Show possible walkpoint configurations
            DisplayConfigurations();

            if (EditorGUI.EndChangeCheck()) // Check if there were changes
            {
                Undo.RecordObject(node, "Modify Node Configurations"); // Record the object for undo
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayNeighborsFoldout()
        {
            // Panel color
            GUI.backgroundColor = Color.white;

            EditorGUILayout.LabelField("Neighbors", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical

            node.ShowNeighborsFoldout = EditorGUILayout.Foldout(node.ShowNeighborsFoldout, $"{node.Neighbors.Count} Neighbors:");

            if (node.ShowNeighborsFoldout)
            {
                for (int i = 0; i < node.Neighbors.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    node.Neighbors[i] = (NavNode)EditorGUILayout.ObjectField(node.Neighbors[i], typeof(NavNode), allowSceneObjects: true);

                    GUI.backgroundColor = Color.red;
                    // Remove neighbor button
                    if (GUILayout.Button("Remove node", GUILayout.Width(150)))
                    {
                        Undo.RecordObject(node, $"Remove neighbor {node.Neighbors[i]}"); // Record the object for undo
                        node.RemoveNeighbor(node.Neighbors[i]);
                    }
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.EndHorizontal();
                }

                // Add new neighbor button
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Add neighbor"))
                {
                    node.AddNeighbor(null);
                }
                GUI.backgroundColor = Color.white;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DisplayPossibleNeighborsFoldout()
        {
            // Panel color
            Color panelColor = new Color(0.9f, 0.4f, 0); // Orange color
            GUI.backgroundColor = panelColor;

            List<NavNode> possibleNodes = node.GetPossibleNeighbors();

            EditorGUILayout.LabelField($"{possibleNodes.Count} Possible Neighbors", EditorStyles.boldLabel);

            if (possibleNodes.Count == 0) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical

            EditorGUI.indentLevel++;

            node.ShowPossibleNeighborsFoldout = EditorGUILayout.Foldout(node.ShowPossibleNeighborsFoldout, $"{possibleNodes.Count} Possible Neighbors:");

            if (node.ShowPossibleNeighborsFoldout)
            {
                for (int i = 0; i < possibleNodes.Count; i++)
                {
                    if (possibleNodes[i] == null) continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(possibleNodes[i], typeof(NavNode), allowSceneObjects: true);

                    // Remove neighbor button
                    GUI.backgroundColor = Color.cyan;
                    if (GUILayout.Button("Add node", GUILayout.Width(150)))
                    {
                        Undo.RecordObject(node, $"Remove neighbor {possibleNodes[i]}"); // Record the object for undo
                        EditorUtility.SetDirty(node);
                        node.AddNeighbor(possibleNodes[i]);
                    }
                    GUI.backgroundColor = panelColor;
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DisplayConfigurations()
        {
            // Panel color
            GUI.backgroundColor = Color.white;

            EditorGUI.BeginChangeCheck(); // Start change check (to update Scene Guizmos)

            EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical
            EditorGUILayout.LabelField("Walkpoint", EditorStyles.boldLabel);

            node.HasMultipleConfiguration = EditorGUILayout.Toggle("Has multiple configurations", node.HasMultipleConfiguration);

            // Default first Vector3 attribute
            node.Configurations[0].isActive = true; // Always active

            if (node.HasMultipleConfiguration)
            {
                for (int i = 0; i < node.Configurations.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField($"Configuration {i}", GUILayout.Width(120));
                        if (i > 0)
                        {
                            EditorGUILayout.LabelField($"Active", GUILayout.Width(70));
                            node.Configurations[i].isActive = EditorGUILayout.Toggle(node.Configurations[i].isActive);
                        }

                        // If the configuration is active, we show the Apply button
                        if (node.Configurations[i].isActive)
                        {
                            if (GUILayout.Button("Apply"))
                            {
                                node.ApplyConfiguration(i);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    // If the configuration is active, we allow the offset edition
                    if (node.Configurations[i].isActive)
                    {
                        EditorGUI.indentLevel++;
                        Undo.RecordObject(node, $"Configuration {i} changed"); // Record the object for undo
                        // Vector3 attribute
                        node.Configurations[i].offset = EditorGUILayout.Vector3Field("Offset", node.Configurations[i].offset);
                        EditorGUI.indentLevel--;
                    }
                }
            }
            else
            {
                if (EditorGUI.EndChangeCheck()) // Check if there were changes
                {
                    EditorUtility.SetDirty(target); // Mark the object as dirty to repaint gizmos
                }

                node.ApplyConfiguration(0);
                // Default first Vector3 attribute
                node.Configurations[0].offset = EditorGUILayout.Vector3Field("Offset", node.Configurations[0].offset);
            }
            EditorGUILayout.EndVertical();


            if (EditorGUI.EndChangeCheck()) // Check if there were changes
            {
                EditorUtility.SetDirty(target); // Mark the object as dirty to repaint gizmos
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
