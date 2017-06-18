using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BattleObject), true)]
public class BattleObjectEditor : Editor {
    GUIStyle labelStyle;
    SerializedProperty objectType;
    SerializedProperty collisionType;
    SerializedProperty radius;
    SerializedProperty width;
    SerializedProperty height;

    void OnEnable()
    {
        labelStyle = new GUIStyle();
        labelStyle.fontStyle = FontStyle.Bold;

        objectType = serializedObject.FindProperty("objectType");
        collisionType = serializedObject.FindProperty("collisionType");
        radius = serializedObject.FindProperty("radius");
        width = serializedObject.FindProperty("width");
        height = serializedObject.FindProperty("height");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Battle Object", labelStyle);
        EditorGUILayout.PropertyField(objectType);
        EditorGUILayout.PropertyField(collisionType);

        int typeIndex = collisionType.enumValueIndex + 1;
        if (typeIndex == (int)CollisionType.Circle)
        {
            EditorGUILayout.PropertyField(radius);
        }
        else if(typeIndex == (int)CollisionType.Box)
        {
            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(height);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
