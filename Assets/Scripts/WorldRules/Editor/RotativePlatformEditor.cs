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

        private void OnEnable()
        {
            platform = target as RotativePlatform;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            //Color originalBackgroundColor = GUI.backgroundColor;
            //EditorGUILayout.LabelField("Custom Editor", EditorStyles.boldLabel);

            GUILayout.Space(10);

            platform.inputEnabled = EditorGUILayout.Toggle("Input Enabled", platform.inputEnabled);
            platform.SpinAxis = (RotateAxis) EditorGUILayout.EnumPopup("Spin Axis", platform.SpinAxis);

            GUILayout.Space(10);

            SerializedProperty configurationsArray = serializedObject.FindProperty("configurations");

            if (configurationsArray != null && configurationsArray.isArray)
            {
                // if is empty we create a 4 element array
                if (configurationsArray.arraySize == 0) configurationsArray.arraySize = 4;

                for (int i = 0; i < configurationsArray.arraySize; i++)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox); // - Vertical

                    EditorGUILayout.BeginHorizontal(); //                    

                    //EditorGUILayout.LabelField("Configuration " + i + ":", EditorStyles.boldLabel);

                    //if (GUILayout.Button("Establish", GUILayout.Width(100)))
                    if (GUILayout.Button("Configuration " + i.ToString()))
                    {
                        // Button logic
                        Vector3 rotationVector = Vector3.zero;
                        rotationVector[(int)platform.SpinAxis] = i * 90;

                        platform.transform.rotation = Quaternion.Euler(rotationVector);

                        platform.ApplyConfiguration();

                    }
                    EditorGUILayout.EndHorizontal(); //

                    //SerializedProperty linkers = configurationsArray.GetArrayElementAtIndex(i)

                    EditorGUILayout.PropertyField(configurationsArray.GetArrayElementAtIndex(i)); // Default drawing

                    EditorGUILayout.EndVertical(); // - Vertical

                    GUILayout.Space(10);
                }

            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}

#endif
