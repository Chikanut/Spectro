using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(LocationsConfig))]
public class LocationsConfigEditor : Editor
{
    ReorderableList _locationsList;

    private int _focused = 0;

    private void OnEnable()
    {
        InitLocationsList();
    }


    void InitLocationsList()
    {
        _locationsList = new ReorderableList(serializedObject,
            serializedObject.FindProperty("LocationConfigs"),
            true, true, true, true);

        _locationsList.drawElementCallback =
            (rect, index, isActive, isFocused) =>
            {
                var element = _locationsList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight),
                    "Location");

                EditorGUI.PropertyField(
                    new Rect(125, rect.y, GUILayoutUtility.GetLastRect().width-120, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("LocationType"), GUIContent.none);

                var line = 0f;

                line += 1.3f;

                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y + (line * EditorGUIUtility.singleLineHeight), 120,
                        EditorGUIUtility.singleLineHeight),
                    "Patterns");

                EditorGUI.PropertyField(
                    new Rect(125, rect.y + (line * EditorGUIUtility.singleLineHeight), GUILayoutUtility.GetLastRect().width-120,
                        EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("PatternsInfo"), GUIContent.none);

            };

        _locationsList.elementHeightCallback = (int index) =>
            EditorGUIUtility.singleLineHeight * 2.5f;

        _locationsList.onCanRemoveCallback = list => list.count > 1;

        _locationsList.onRemoveCallback = list =>
        {
            if (EditorUtility.DisplayDialog("Warning!",
                "Are you sure you want to delete this difficulty entry?", "Yes", "No"))
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            }
        };

        _locationsList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(
                new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight),
                new GUIContent("Locations info"));
        };
        _locationsList.onSelectCallback = list => { _focused = list.index; };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        _locationsList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
