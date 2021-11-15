//Create a folder and call it "Editor" if one doesn't already exist. Place this script in it.
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// Create a 180 degrees wire arc with a ScaleValueHandle attached to the disc
// lets you visualize some info of the transform

namespace Packages.Utils.Extensions
{
    [CustomEditor(typeof(SceneWidth))]
    public class SceneWidthInspector : Editor
    {
        void OnSceneGUI()
        {
            Handles.BeginGUI();
        
            GUILayout.BeginArea(new Rect(20, 20, 150, 60));
        
            var rect = EditorGUILayout.BeginVertical();
            GUI.color = Color.yellow;
            GUI.Box(rect, GUIContent.none);
        
            GUI.color = Color.white;
        
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Rotate");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.red;
        
            if (GUILayout.Button("Left")) {
                RotateLeft();
            }
        
            if (GUILayout.Button("Right")) {
                RotateRight();
            }
        
            GUILayout.EndHorizontal();
        
            EditorGUILayout.EndVertical();
        
        
            GUILayout.EndArea();
        
            Handles.EndGUI();
        }
    
        void RotateLeft() {
            (target as Component).transform.Rotate(Vector3.down, 15);
        }
    
        void RotateRight() {
            (target as Component).transform.Rotate(Vector3.down, -15);
        }
    }
}
#endif
