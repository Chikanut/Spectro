using UnityEditor;
using UnityEngine;

namespace Gameplay.AngleRange.Editor
{
    public class AngleRangeConst
    {
        public static class Contents
        {
            public static readonly GUIContent angleRangesLabel = new GUIContent("Angle Ranges");
            public static readonly GUIContent angleRangeLabel = new GUIContent("Angle Range ({0})");

            public static readonly Color proBackgroundColor = new Color32(49, 77, 121, 255);
            public static readonly Color proBackgroundRangeColor = new Color32(25, 25, 25, 128);
            public static readonly Color proColor1 = new Color32(10, 46, 42, 255);
            public static readonly Color proColor2 = new Color32(33, 151, 138, 255);
            public static readonly Color defaultColor1 = new Color32(25, 61, 57, 255);
            public static readonly Color defaultColor2 = new Color32(47, 166, 153, 255);
            public static readonly Color defaultBackgroundColor = new Color32(64, 92, 136, 255);
        }
        
        private const float kSpacingSubLabel = 2.0f;

        public static void MultiDelayedIntField(Rect position, GUIContent[] subLabels, int[] values, float labelWidth)
        {
            int eCount = values.Length;
            float w = (position.width - (eCount - 1) * kSpacingSubLabel) / eCount;
            Rect nr = new Rect(position);
            nr.width = w;
            float t = EditorGUIUtility.labelWidth;
            int l = EditorGUI.indentLevel;
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = 0;
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = EditorGUI.DelayedIntField(nr, subLabels[i], values[i]);
                nr.x += w + kSpacingSubLabel;
            }

            EditorGUIUtility.labelWidth = t;
            EditorGUI.indentLevel = l;
        }
    }
}
