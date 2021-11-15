using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gameplay.AngleRange.Editor
{
    public class AngleRangePanel : IAngleRangeCache
    {
        private Object _target;
        private AngleRangeController controller;

        private const int kInvalidMinimum = -1;
        [SerializeField] private int m_SelectedIndex;

        public int selectedIndex
        {
            get { return m_SelectedIndex; }
            set { m_SelectedIndex = value; }
        }

        [SerializeField] private float m_PreviewAngle = 0f;

        public float previewAngle
        {
            get { return m_PreviewAngle; }
            set
            {
                m_PreviewAngle = value;
                SessionState.SetFloat("AngleRange/PreviewAngle/" + _target.GetInstanceID(), value);
            }
        }

        private AngleRange m_CurrentAngleRange;
        private Rect m_AngleRangeRect;

        public List<AngleRange> angleRanges
        {
            get
            {
                if (_target == null)
                    return new List<AngleRange>();

                Debug.Assert(_target != null);
                return ((IAnleRangeUser) _target).AngleRanges;
            }
        }

        public Action<int> OnRangeChanged;
        public Action<int> OnRangeAdded;
        public Action<int> OnRangeRemoved;

        public AngleRangePanel(Object target)
        {
            _target = target;

            m_PreviewAngle = SessionState.GetFloat("AngleRange/PreviewAngle/" + target.GetInstanceID(), m_PreviewAngle);

            SetupAngleRangeController();
        }

        public void DrawHeader(GUIContent content)
        {
            EditorGUILayout.LabelField(content, EditorStyles.boldLabel);
        }

        public void Draw()
        {
            EditorGUILayout.Space();
            DrawHeader(AngleRangeConst.Contents.angleRangesLabel);

            DoRangesGUI();
            DoRangeInspector();
            DoCreateRangeButton();
        }


        private void SetupAngleRangeController()
        {
            var radius = 125f;
            var angleOffset = -90f;
            var color1 = AngleRangeConst.Contents.defaultColor1;
            var color2 = AngleRangeConst.Contents.defaultColor2;

            if (!EditorGUIUtility.isProSkin)
            {
                color1 = AngleRangeConst.Contents.proColor1;
                color2 = AngleRangeConst.Contents.proColor2;
            }

            controller = new AngleRangeController();
            controller.view = new AngleRangeView();
            controller.cache = this;
            controller.radius = radius;
            controller.angleOffset = angleOffset;
            controller.gradientMin = color1;
            controller.gradientMid = color2;
            controller.gradientMax = color1;
            controller.snap = true;
            controller.selectionChanged += OnSelectionChange;
            controller.onRangeAdded = OnAdded;
            controller.onRangeRemoved = OnRemoved;

            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            EditorApplication.delayCall += () =>
            {
                m_CurrentAngleRange = controller.selectedAngleRange;
                OnRangeChanged?.Invoke(controller.selectedIndex);
            };
        }
        
        private void OnAdded(int index)
        {
            EditorApplication.delayCall += () =>
            {
                OnRangeAdded?.Invoke(index);
            };
        }
        
        private void OnRemoved(int index)
        {
            EditorApplication.delayCall += () =>
            {
                OnRangeRemoved?.Invoke(index);
            };
        }
        

        public void RegisterUndo(string name)
        {
            Undo.RegisterCompleteObjectUndo(_target, _target.name);
        }

        private void DoRangeInspector()
        {
            var start = 0f;
            var end = 0f;
            var order = 0;

            if (m_CurrentAngleRange != null)
            {
                start = m_CurrentAngleRange.start;
                end = m_CurrentAngleRange.end;
                order = m_CurrentAngleRange.order;
            }

            using (new EditorGUI.DisabledGroupScope(m_CurrentAngleRange == null))
            {
                DrawHeader(new GUIContent(string.Format(AngleRangeConst.Contents.angleRangeLabel.text, (end - start))));

                EditorGUIUtility.labelWidth = 0f;
                EditorGUI.BeginChangeCheck();

                RangeField(ref start, ref end, ref order);

                if (EditorGUI.EndChangeCheck() && m_CurrentAngleRange != null)
                {
                    RegisterUndo("Set Range");

                    m_CurrentAngleRange.order = order;
                    controller.SetRange(m_CurrentAngleRange, start, end);

                    if (start >= end)
                        controller.RemoveInvalidRanges();
                }
            }
        }

        private void RangeField(ref float start, ref float end, ref int order)
        {
            var values = new int[] {Mathf.RoundToInt(-start), Mathf.RoundToInt(-end), order};
            var labels = new GUIContent[] {new GUIContent("Start"), new GUIContent("End"), new GUIContent("Order")};

            var position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginChangeCheck();
            AngleRangeConst.MultiDelayedIntField(position, labels, values, 40f);
            if (EditorGUI.EndChangeCheck())
            {
                start = -1f * values[0];
                end = -1f * values[1];
                order = values[2];
            }
        }

        private void DoCreateRangeButton()
        {
            if (selectedIndex != kInvalidMinimum && angleRanges.Count != 0)
                return;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Create Range", GUILayout.MaxWidth(100f)))
            {
                RegisterUndo("Create Range");
                controller.CreateRange();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }


        private void DoRangesGUI()
        {
            var radius = controller.radius;

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            var rect = EditorGUILayout.GetControlRect(false, radius * 2f);

            if (Event.current.type == EventType.Repaint)
                m_AngleRangeRect = rect;

            {
                //Draw background
                var backgroundColor = AngleRangeConst.Contents.proBackgroundColor;
                var backgroundRangeColor = AngleRangeConst.Contents.proBackgroundRangeColor;

                if (!EditorGUIUtility.isProSkin)
                {
                    backgroundColor = AngleRangeConst.Contents.defaultBackgroundColor;
                    backgroundRangeColor.a = 0.1f;
                }

                var c = Handles.color;
                Handles.color = backgroundRangeColor;
                AngleRangeUtility.DrawSolidArc(rect.center, Vector3.forward, Vector3.right, 360f, radius,
                    AngleRangeGUI.kRangeWidth);
                Handles.color = backgroundColor;
                Handles.DrawSolidDisc(rect.center, Vector3.forward, radius - AngleRangeGUI.kRangeWidth + 1f);
                Handles.color = c;
            }


            // { TEXTURE DRAWING INSIDE ANGLE CIRCLE
                //Draw fill texture and sprite preview
                // SpriteShapeHandleUtility.DrawTextureArc(
                //     m_FillTextureProp.objectReferenceValue as Texture, 100.0f,
                //     rect.center, Vector3.forward, Quaternion.AngleAxis(m_PreviewAngle, Vector3.forward) * Vector3.right, 180f,
                //     radius - AngleRangeGUI.kRangeWidth);
                //
                // var rectSize = Vector2.one * (radius - AngleRangeGUI.kRangeWidth) * 2f;
                // rectSize.y *= 0.33f;
                // var spriteRect = new Rect(rect.center - rectSize * 0.5f, rectSize);
                // DrawSpritePreview(spriteRect);
                // HandleSpritePreviewCycle(spriteRect);
            // }

            controller.rect = m_AngleRangeRect;
            controller.OnGUI();


            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
    }
}
