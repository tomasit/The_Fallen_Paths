using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

#if (UNITY_EDITOR)
[CustomEditor(typeof(SubLevelChange))]
public class SubLevelChangeGUI : Editor
{
    private SerializedProperty _tilemap;
    private SerializedProperty _player;
    private SerializedProperty _soundType;
    private SerializedProperty _transitionScreen;

    private void OnEnable()
    {
        _player = serializedObject.FindProperty("_player");
        _tilemap = serializedObject.FindProperty("_grid");
        _soundType = serializedObject.FindProperty("_soundType");
        _transitionScreen = serializedObject.FindProperty("_transitionScreen");
    }

    public override void OnInspectorGUI()
    {
        SubLevelChange place = (SubLevelChange)target;

        EditorGUILayout.PropertyField(_tilemap);
        EditorGUILayout.PropertyField(_player);
        EditorGUILayout.PropertyField(_soundType);

        place._interactOnStart = GUILayout.Toggle(place._interactOnStart, "Interact on start");

        if ((place._translateChange = GUILayout.Toggle(place._translateChange, "On change translate")))
        {
            place._fadeChange = false;
            place._translateSpeed = EditorGUILayout.FloatField("Translate speed", place._translateSpeed);
        }
        else
            place._fadeChange = true;

        if ((place._fadeChange = GUILayout.Toggle(place._fadeChange, "On change fade")))
        {
            place._translateChange = false;
            EditorGUILayout.PropertyField(_transitionScreen);
            GUILayout.Label("Description");
            place._quoteName = GUILayout.TextArea(place._quoteName, 200);
            
        }
        else
            place._translateChange = true;

        serializedObject.ApplyModifiedProperties();
    }
}
#endif