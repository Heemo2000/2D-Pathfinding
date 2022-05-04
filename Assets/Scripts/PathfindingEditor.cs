using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathfindingData))]
[CanEditMultipleObjects]
public class PathfindingEditor : Editor
{
    SerializedProperty width;
    SerializedProperty height;
    SerializedProperty cellSize;

    SerializedProperty shouldUsePenaltyCost;
    SerializedProperty terrainTypes;

    SerializedProperty obstaclePenaltyCost;

    private void OnEnable() {
        width = serializedObject.FindProperty("width");
        height = serializedObject.FindProperty("height");
        cellSize = serializedObject.FindProperty("cellSize");
        shouldUsePenaltyCost = serializedObject.FindProperty("shouldUsePenaltyCost");
        terrainTypes = serializedObject.FindProperty("terrainTypes");
        obstaclePenaltyCost = serializedObject.FindProperty("obstaclePenaltyCost");
    }
    public override void OnInspectorGUI()
    {
        
        serializedObject.Update();
        EditorGUILayout.PropertyField(width);
        EditorGUILayout.PropertyField(height);
        EditorGUILayout.PropertyField(cellSize);
        EditorGUILayout.PropertyField(shouldUsePenaltyCost);
        if(shouldUsePenaltyCost.boolValue == true)
        {
            EditorGUILayout.PropertyField(terrainTypes);
            EditorGUILayout.PropertyField(obstaclePenaltyCost);
        }
        serializedObject.ApplyModifiedProperties();
        
    }
}
