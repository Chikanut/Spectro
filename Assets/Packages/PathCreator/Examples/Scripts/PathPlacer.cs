using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace PathCreation.Examples {

    [ExecuteInEditMode]
    public class PathPlacer : PathSceneTool {

        public SpriteRenderer _dotImage;
        public float spacing = 3;
    
        const float minSpacing = .1f;
        
        List<SpriteRenderer> _points = new List<SpriteRenderer>();

        private void Awake()
        {
            _points = GetComponentsInChildren<SpriteRenderer>().ToList();
        }

        public void Generate ()
        {
            if (pathCreator == null || _dotImage == null) return;
            
            DestroyObjects ();

            var path = pathCreator.path;
            float dst = 0;
            
            spacing = Mathf.Max(minSpacing, spacing);

            while (dst < path.length)
            {
                var point = path.GetPointAtDistance (dst);
                var newPoint = Instantiate (_dotImage, point, Quaternion.identity, transform);
                _points.Add(newPoint);
                dst += spacing;
            }
        }

        private Sequence _showingSequence;
        
        public void Show(bool show)
        {
            _showingSequence?.Kill();
            _showingSequence = DOTween.Sequence();
            
            for (int i = 0; i < _points.Count; i++)
            {
                if(_points[i] == null) continue;
                _showingSequence.Insert(0, _points[i].DOFade(show ? 1 : 0, 1));
            }

            _showingSequence.Play();
        }

        void DestroyObjects ()
        {
            for (int i = _points.Count - 1; i >= 0; i--)
            {
                if(_points[i] != null)
                    DestroyImmediate(_points[i].gameObject, false);
            }

            _points.Clear();
        }

        protected override void PathUpdated ()
        {
            if (pathCreator != null) 
                Generate ();
        }
    }
}