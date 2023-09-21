using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Monument.World;
using static UnityEngine.GraphicsBuffer;
using Codice.Client.Common;
using static Rotable;

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
            node.GlobalWalkpoint = EditorGUILayout.Toggle("Global Walkpoint", node.GlobalWalkpoint);

            DisplayNeighborsFoldout();

            DisplayPossibleNeighborsFoldout();

            DisplayConfigurations();

            //TODO: show possible walkpoint configurations
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("RotatorHandle"));

            //GUILayout.Space(10);
        }

        private void DisplayNeighborsFoldout()
        {
            EditorGUILayout.LabelField("Neighbors", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical

            node.ShowNeighborsFoldout = EditorGUILayout.Foldout(node.ShowNeighborsFoldout, $"{node.Neighbors.Count} Neighbors:");

            if (node.ShowNeighborsFoldout)
            {
                for (int i = 0; i < node.Neighbors.Count; i++)
                {
                    if (node.Neighbors[i] == null) continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(node.Neighbors[i], typeof(NavNode), allowSceneObjects: true);

                    GUI.backgroundColor = Color.red;
                    // Remove neighbor button
                    if (GUILayout.Button("Remove node", GUILayout.Width(150)))
                    {
                        Undo.RecordObject(node, $"Remove neighbor {node.Neighbors[i]}"); // Record the object for undo
                        EditorUtility.SetDirty(node);
                        node.RemoveNeighbor(node.Neighbors[i]);
                    }
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DisplayPossibleNeighborsFoldout()
        {
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
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DisplayConfigurations()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical
            EditorGUILayout.LabelField("Walkpoint", EditorStyles.boldLabel);
            node.HasMultipleConfiguration = EditorGUILayout.Toggle("Has multiple configurations", node.HasMultipleConfiguration);

            // Default first Vector3 attribute
            node.Configurations[0].Item1 = true; // Always active

            if (node.HasMultipleConfiguration)
            {
                for (int i = 0; i < node.Configurations.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField($"Configuration {i} ");
                        if (i > 0) node.Configurations[i].Item1 = EditorGUILayout.Toggle("Active", node.Configurations[i].Item1);
                    }
                    EditorGUILayout.EndHorizontal();

                    if (node.Configurations[i].Item1)
                    {
                        // Vector3 attribute
                        node.Configurations[i].Item2 = EditorGUILayout.Vector3Field("Offset", node.Configurations[i].Item2);
                    }
                }
            }
            else
            {
                // Default first Vector3 attribute
                node.Configurations[0].Item2 = EditorGUILayout.Vector3Field("Offset", node.Configurations[0].Item2);
            }
            EditorGUILayout.EndVertical();
        }
    }
}
