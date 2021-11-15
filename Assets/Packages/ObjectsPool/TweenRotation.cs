using System;
using System.Collections;
using DG.Tweening;
using Packages.Utils.Extensions;
using UnityEngine;

public class TweenRotation : MonoBehaviour, TweenObject
{
    [Serializable]
    public class RotationSettings
    {
        [Range(0.1f, 10f)]
        public float Duration;
        public bool IsForward = true;
        public bool Loop;
        public bool EnableCycle;
        public Ease Ease;
        public Vector3[] Rotations;
    }

    [SerializeField]
    public RotationSettings Settings;

    private Action _onFinish;
    
    private Sequence _rotationSequence;

    public void Init(RotationSettings settings)
    {
        OnDisable();

        Settings = settings;

        OnEnable();
    }

    private void OnEnable()
    {
        if (Settings?.Rotations == null || Settings.Rotations.Length < 2) return;

        _rotationSequence = DOTween.Sequence();
        _rotationSequence.SetAutoKill(false);
        
        PlayTween();
    }

    private void OnDisable() {
        if (_rotationSequence == null)
            return;
        
        _rotationSequence.Kill();
        _rotationSequence.Rewind();
        
        _rotationSequence = null;
    }

    public void PlayTween(Action onFinish = null) {
        _onFinish = onFinish;

        if (Settings?.Rotations == null || Settings.Rotations.Length < 2) return;
        
        var segmentDuration = Settings.Duration / Settings.Rotations.Length;
        
        StartCoroutine(WaitForTheEndOfFrame(() => {
            MoveToSegment(1, segmentDuration);
        }));
    }

    private IEnumerator WaitForTheEndOfFrame(Action onFinish = null) {
        yield return new WaitForEndOfFrame();
        onFinish?.Invoke();
    }
    
    private void MoveToSegment(int index, float duration)
    {
        if(Settings?.Rotations == null || duration == 0 || Settings.Rotations.Length <= index)
            return;
        
        _rotationSequence = DOTween.Sequence();
        _rotationSequence.Append(transform.DOLocalRotate(Settings.Rotations[index], duration).SetEase(Settings.Ease));
        
        _rotationSequence.onComplete = () =>
        {
            
            if (!Settings.EnableCycle && (Settings.IsForward && index >= Settings.Rotations.Length - 1 || !Settings.IsForward && index < 1))
            {
                _onFinish?.Invoke();
                return;
            }
            
            MoveToSegment(FindNextSegment(index), duration);
        };
    }

    private int FindNextSegment(int currentSegmentIndex)
    {
        if (Settings.IsForward) {
            
            if (currentSegmentIndex < Settings.Rotations.Length - 1)
            {
                currentSegmentIndex++;
            }
            else 
            {
                if (Settings.Loop) 
                    currentSegmentIndex = 0;
                else
                {
                    Settings.IsForward = false;
                    currentSegmentIndex--;
                }
            }
        }
        else {
            if (currentSegmentIndex > 0)
                currentSegmentIndex--;
            else
            {
                if (Settings.Loop)
                    currentSegmentIndex = Settings.Rotations.Length - 1;
                else
                {
                    Settings.IsForward = true;
                    currentSegmentIndex++;
                }
            }
        }
        
        return currentSegmentIndex;
    }

    private Vector3 _offset;
    
    private void OnDrawGizmos()
    {
        if (Settings == null || Settings.Rotations == null || Settings.Rotations.Length < 2) return;

        Gizmos.color = Color.cyan;
        
        for (int i = 0; i < Settings.Rotations.Length; i++)
        {
            var dir = (Quaternion.Euler(Settings.Rotations[i]) * Vector2.right * Mathf.Sign(transform.localScale.x)).normalized;
            
            Gizmos.DrawLine(transform.position, transform.position + dir);

            if (i <= 0)
                continue;
            
            var prevDir = (Quaternion.Euler(Settings.Rotations[i - 1]) * Vector2.right * Mathf.Sign(transform.localScale.x))
                .normalized;
                
            DrawArrow.ForGizmo(transform.position + prevDir , transform.position + dir);
        }
    }
}
