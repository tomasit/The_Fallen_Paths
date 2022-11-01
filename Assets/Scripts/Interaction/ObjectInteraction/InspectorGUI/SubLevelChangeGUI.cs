using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(SubLevelChange))]
public class SubLevelChangeGUI : Editor
{
    private SerializedProperty _tilemap;
    private SerializedProperty _image;
    private SerializedProperty _text;

    private void OnEnable()
    {
        _tilemap = serializedObject.FindProperty("_grid");
        _image = serializedObject.FindProperty("_fadeImage");
        _text = serializedObject.FindProperty("_tmproUGUI");
    }

    public override void OnInspectorGUI()
    {
        SubLevelChange place = (SubLevelChange)target;

        EditorGUILayout.PropertyField(_tilemap);

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
            EditorGUILayout.PropertyField(_image);
            EditorGUILayout.PropertyField(_text);
            GUILayout.Label("Description");
            place._fadeDescription = GUILayout.TextArea(place._fadeDescription, 200);
            place._descriptionDuration = EditorGUILayout.FloatField("Description duration", place._descriptionDuration);
            place._fadeDuration = EditorGUILayout.FloatField("Fade duration", place._fadeDuration);
        }
        else
            place._translateChange = true;

        serializedObject.ApplyModifiedProperties();
    }
}
