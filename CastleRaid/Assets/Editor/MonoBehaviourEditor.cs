using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(MonoBehaviour), true)]
public class MonoBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            Type type = target.GetType();

            foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                object[] attributes = method.GetCustomAttributes(typeof(AddEditorInvokeButtonAttribute), true);
                if (attributes.Length > 0)
                {
                    if (GUILayout.Button("Invoke: " + method.Name))
                    {
                        ((MonoBehaviour)target).Invoke(method.Name, 0f);
                    }
                }
            }
        }
    }
}
