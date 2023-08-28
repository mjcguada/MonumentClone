#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Rotable;

namespace Monument.World
{
    [CustomEditor(typeof(RotativePlatform))]
    public class RotativePlatformEditor : Editor
    {
        private RotativePlatform platform;

        // Highlight selected configuration
        private int selectedConfiguration = -1;
        private Color selectedColor = new Color(0.4f, 0.9f, 0.6f);

        private void OnEnable()
        {
            platform = target as RotativePlatform;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(10);

            platform.SpinAxis = (RotateAxis)EditorGUILayout.EnumPopup("Spin Axis", platform.SpinAxis);

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Optional", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RotatorHandle"));

            GUILayout.Space(10);

            // Configuration buttons
            SerializedProperty configurationsArray = serializedObject.FindProperty("configurations");

            // if is empty we create a 4 element array (4 possible rotation positions 360/90 = 4)
            if (configurationsArray.arraySize == 0) configurationsArray.arraySize = 4;

            for (int i = 0; i < configurationsArray.arraySize; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical

                EditorGUILayout.BeginHorizontal(); // - Button Space

                // Element color (white default, green for selected one)
                GUI.backgroundColor = selectedConfiguration == i ? selectedColor : Color.white;

                // Create button
                if (GUILayout.Button("Show Configuration " + i.ToString()))
                {
                    // Focus on platform gameObject
                    Bounds bounds = new Bounds(platform.transform.position, platform.transform.localScale * 3f);
                    SceneView.lastActiveSceneView.Frame(bounds, false);

                    // Rotate the platform to the current configuration index
                    Vector3 rotationVector = Vector3.zero;
                    rotationVector[(int)platform.SpinAxis] = i * 90;
                    platform.transform.rotation = Quaternion.Euler(rotationVector);

                    // Apply linkers of the configuration
                    platform.ApplyConfiguration();

                    // Set selected configuration
                    selectedConfiguration = i;
                }
                EditorGUILayout.EndHorizontal(); // - Button Space

                // Get linkers array
                SerializedProperty linkersArray = configurationsArray.GetArrayElementAtIndex(i).FindPropertyRelative("Linkers");

                // We only show the linkers content of the selected configuration
                if (selectedConfiguration == i)
                {
                    // Show every linker from the selected configuration
                    for (int j = 0; j < linkersArray.arraySize; j++)
                    {
                        GUILayout.Space(2); // Spacing between linkers

                        SerializedProperty linkerProperty = linkersArray.GetArrayElementAtIndex(j);

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical

                        EditorGUILayout.ObjectField(linkerProperty.FindPropertyRelative("NodeA"));
                        EditorGUILayout.ObjectField(linkerProperty.FindPropertyRelative("NodeB"));

                        EditorGUILayout.BeginHorizontal();  //--------------------------------
                        EditorGUILayout.PropertyField(linkerProperty.FindPropertyRelative("areLinked"));

                        GUI.backgroundColor = Color.red;

                        if (GUILayout.Button("Remove Linker", GUILayout.Width(120)))
                        {
                            linkersArray.DeleteArrayElementAtIndex(j);
                            break; // Exit the loop to prevent issues with indexing
                        }

                        EditorGUILayout.EndHorizontal();    //--------------------------------

                        GUI.backgroundColor = selectedColor;
                        EditorGUILayout.EndVertical(); // - Vertical
                    }

                    // Show Add Linker button
                    if (GUILayout.Button("Add Linker"))
                    {
                        linkersArray.arraySize++;
                    }
                }
                else // Print number of configurations
                {
                    EditorGUILayout.LabelField("Number of linkers: " + linkersArray.arraySize.ToString());
                }

                EditorGUILayout.EndVertical(); // - Vertical

                GUILayout.Space(10); // Space between configurations
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
