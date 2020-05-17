using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using GUIEditor = UnityEditor.EditorGUILayout;
#endif
namespace SPD.Audio
{
#if UNITY_EDITOR
    [CustomEditor(typeof(BankEntry)), CanEditMultipleObjects]
    public class GameSoundsBankEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //SerializedObject Bank = new SerializedObject(target);
            serializedObject.Update();
            GUIEditor.BeginVertical("TextArea");
            GUIEditor.Slider(serializedObject.FindProperty("Volume"), 0.0f, 1.0f);
            if (serializedObject.FindProperty("Volume").floatValue <= 0.0f)
            {
                GUIEditor.HelpBox("The volume value is 0.0 \r\nSet the value for volume.", MessageType.Warning);
            }
            GUIEditor.Slider(serializedObject.FindProperty("Pitch"), 0.0f, 3.0f);
            if (serializedObject.FindProperty("Pitch").floatValue <= 0.0f)
            {
                GUIEditor.HelpBox("The pitch value is 0.0! \r\nSet the value to '1.0f'", MessageType.Error);
            }
            GUIEditor.PropertyField(serializedObject.FindProperty("RandomClip"));
            GUIEditor.EndVertical();

            if (serializedObject.FindProperty("RandomClip").boolValue)
            {
                if (serializedObject.FindProperty("Clip").objectReferenceValue != null)
                {
                    serializedObject.FindProperty("Clip").objectReferenceValue = null;
                }

                if (!serializedObject.FindProperty("Clips").FindPropertyRelative("Array.size").hasMultipleDifferentValues)
                {
                    GUIEditor.BeginVertical("TextArea");
                    GUIEditor.BeginVertical("Box");
                    SerializedProperty SizeClips = serializedObject.FindProperty("Clips").FindPropertyRelative("Array.size");
                    GUIEditor.PropertyField(SizeClips);
                    GUIEditor.EndVertical();

                    if (serializedObject.FindProperty("Clips").arraySize <= 0)
                    {
                        GUIEditor.HelpBox("Set the size of the list of audio files!", MessageType.Error);
                    }
                    else
                    {
                        EditorGUI.indentLevel++;
                        for (int i = 0; i < SizeClips.intValue; i++)
                        {
                            if (serializedObject.FindProperty("Clips").GetArrayElementAtIndex(i).objectReferenceValue == null)
                            {
                                GUIEditor.PropertyField(serializedObject.FindProperty("Clips").GetArrayElementAtIndex(i));
                                GUIEditor.HelpBox("The field with the audio clip is empty! \r\nSet the value to the field!", MessageType.Error);
                            }
                            else
                            {
                                GUIEditor.PropertyField(serializedObject.FindProperty("Clips").GetArrayElementAtIndex(i));
                            }
                        }
                        EditorGUI.indentLevel--;
                    }

                    GUIEditor.EndVertical();
                }
                else
                {
                    GUIEditor.HelpBox("Unable to edit multiple clips at once.", MessageType.Info);
                }
            }
            else
            {
                if (serializedObject.FindProperty("Clips").arraySize > 0)
                {
                    serializedObject.FindProperty("Clips").arraySize = 0;
                }
                GUIEditor.BeginVertical("TextArea");
                GUIEditor.PropertyField(serializedObject.FindProperty("Clip"));
                if (serializedObject.FindProperty("Clip").objectReferenceValue == null)
                {
                    GUIEditor.HelpBox("The field with the audio clip is empty! \r\nSet the value to the field!", MessageType.Error);
                }
                GUIEditor.EndVertical();
            }
            GUIEditor.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}