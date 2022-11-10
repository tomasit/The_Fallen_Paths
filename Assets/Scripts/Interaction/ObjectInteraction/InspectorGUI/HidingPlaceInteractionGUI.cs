using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

#if (UNITY_EDITOR)
[CustomEditor(typeof(HidingPlaceInteraction))]
public class HidingPlaceInteractionGUI : Editor
{
    SerializedProperty _hideEvent;
    SerializedProperty _lightEvent;


    void OnEnable()
    {
        _hideEvent = serializedObject.FindProperty("_hideEvent");
        _lightEvent = serializedObject.FindProperty("_lightEvent");
    }

    public override void OnInspectorGUI()
    {
        HidingPlaceInteraction place = (HidingPlaceInteraction)target;

        EditorGUILayout.PropertyField(_hideEvent);

        if ((place._isLitPlace = GUILayout.Toggle(place._isLitPlace, "Is Lit Place")))
        {
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_lightEvent);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
