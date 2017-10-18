using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{/*
    private ReorderableList audioItemList;

    void OnEnable()
    {
        AudioManager script = (AudioManager)target;

        List<float> elementHeights = new List<float>();

        audioItemList = new ReorderableList(serializedObject, serializedObject.FindProperty("_audioItemSettings"), true, true, true, true)
        {
            headerHeight = 2,
            drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty element = audioItemList.serializedProperty.GetArrayElementAtIndex(index);

                rect.y += 2;
                rect.height = 50;

                Color previousColor = GUI.color;

                float height = EditorGUIUtility.singleLineHeight * 1.25f;

                if (active)
                {
                    height = EditorGUIUtility.singleLineHeight * 5f;
                }

                try
                {
                    elementHeights[index] = height;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.LogWarning(e.Message);
                }
                finally
                {
                    float[] floats = elementHeights.ToArray();
                    Array.Resize(ref floats, audioItemList.serializedProperty.arraySize);
                    elementHeights = new List<float>(floats);
                }

                if (active)
                {
                    float space = 0f;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y + space, rect.width, EditorGUIUtility.singleLineHeight), 
                        element.FindPropertyRelative("name"), GUIContent.none);
                    space += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y + space, rect.width, EditorGUIUtility.singleLineHeight), 
                        element.FindPropertyRelative("clip"), GUIContent.none);
                    space += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y + space, rect.width, EditorGUIUtility.singleLineHeight), 
                        element.FindPropertyRelative("mixerGroup"), GUIContent.none);
                }
                else
                {
                    EditorGUI.LabelField(rect, element.FindPropertyRelative("name").stringValue);
                }

                GUI.color = previousColor;
            },
            elementHeightCallback = (index) => 
            {
                Repaint();
                float height = 0f;
                try
                {
                    height = elementHeights[index];
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.LogWarning(e.Message);
                }
                finally
                {
                    float[] floats = elementHeights.ToArray();
                    Array.Resize(ref floats, audioItemList.serializedProperty.arraySize);
                    elementHeights = new List<float>(floats);
                }
                return elementHeights[index];
            },
            drawHeaderCallback = (rect) => 
            {
                EditorGUI.LabelField(rect, "Audio");
            },
            onAddCallback = (list) =>
            {
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;
            },
            onRemoveCallback = (list) =>
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            },
        };
    }

    void OnDisable()
    {
        audioItemList = null;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        AudioManager script = (AudioManager)target;

        audioItemList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }*/
}
