using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(LevelsConfig))]
public class LevelConfigsEditor : Editor {
    
    private ReorderableList _list;

    private void OnEnable() {   
        _list = new ReorderableList(serializedObject, 
            serializedObject.FindProperty("LevelConfigs"), 
            true, true, true, true);

        _list.drawElementCallback = 
            (rect, index, isActive, isFocused) => {
                var element = _list.serializedProperty.GetArrayElementAtIndex(index);  
                rect.y += 2;
                
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, GUILayoutUtility.GetLastRect().width-30, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("SerializedLevel"), GUIContent.none);        
  
            };

        _list.onCanRemoveCallback = list => list.count > 1;
        _list.onRemoveCallback = list => {
            if (EditorUtility.DisplayDialog("Warning!", 
                "Are you sure you want to delete this difficulty entry?", "Yes", "No")) {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            }
        };
    }
    
    public override void OnInspectorGUI() {
        serializedObject.Update();
        _list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
