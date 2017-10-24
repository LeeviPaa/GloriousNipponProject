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

        MonoBehaviour script = (MonoBehaviour)target;

        Type type = target.GetType();
        MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        foreach (MethodInfo method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            object[] attributes = method.GetCustomAttributes(typeof(AddEditorInvokeButtonAttribute), true);
            if (attributes.Length > 0)
            {
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length == 0)
                {
                    if (GUILayout.Button("Invoke: " + method.Name))
                    {
                        method.Invoke(script, new object[0]);
                    }
                }
            }
        }
    }
}
