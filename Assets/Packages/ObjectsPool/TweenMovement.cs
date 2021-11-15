using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TweenMovement : MonoBehaviour, TweenObject {

    [Serializable]
    public class MovingSettings
    {
        [Range(1f, 10f)]
        public float Duration;
        public bool IsForward = true;
        public bool Loop;
        public bool EnableCycle;
        public Ease Ease;
        public Vector3[] Positions;
        public bool AnchorPositionOnce;
    }

    [SerializeField]
    public MovingSettings Settings;

    private Action _onFinish;
    
    private Vector3 _startPosition;
    private Sequence _movingSequence;

    private bool _anchored;
    private bool _isInited;

    public void Init(MovingSettings settings)
    {
        OnDisable();

        Settings = settings;
        _isInited = true;

        OnEnable();
    }
    
    private void OnEnable()
    {
        if (Settings?.Positions == null || Settings.Positions.Length < 2) return;

        _movingSequence = DOTween.Sequence();
        _movingSequence.SetAutoKill(false);

        PlayTween();
    }

    private void OnDisable() {
        if (_movingSequence == null) return;
        _movingSequence.Kill();
        _movingSequence.Rewind();

        if (!_isInited)
            return;
        
        Settings = null;
        _isInited = false;
    }

    public void PlayTween(Action onFinish = null) {
        _onFinish = onFinish;

        if(Settings?.Positions == null || Settings.Positions.Length < 2) return;

        float segmentDuration = Settings.Duration / Settings.Positions.Length;
        StartCoroutine(WaitForTheEndOfFrame(() => {
            if ((Settings.AnchorPositionOnce && !_anchored) || !Settings.AnchorPositionOnce)
            {
                _startPosition = transform.localPosition;
                _anchored = true;
            }
            MoveToSegment(1, segmentDuration);
        }));
    }

    private IEnumerator WaitForTheEndOfFrame(Action onFinish = null) {
        yield return new WaitForEndOfFrame();
        onFinish?.Invoke();
    }
    
    private void MoveToSegment(int index, float duration)
    {
        if(Settings?.Positions == null || duration == 0 || Settings.Positions.Length <= index)
            return;
        
        _movingSequence = DOTween.Sequence();
        _movingSequence.Append(transform.DOLocalMove(Settings.Positions[index] + _startPosition, duration).SetEase(Settings.Ease));
        _movingSequence.onComplete = () => 
        {
            if (Settings == null)
                return;
            
            if (!Settings.EnableCycle && (Settings.IsForward && index >= Settings.Positions.Length - 1 ||
                                          !Settings.IsForward && index < 1))
            {
                _onFinish?.Invoke();
                return;
            }

            MoveToSegment(FindNextSegment(index), duration);
        };
    }

    private int FindNextSegment(int currentSegmentIndex) {
        if (Settings.IsForward) {
            if (currentSegmentIndex < Settings.Positions.Length - 1) {
                currentSegmentIndex++;
            }
            else {
                if (Settings.Loop) {
                    currentSegmentIndex = 0;
                }
                else {
                    Settings.IsForward = false;
                    currentSegmentIndex--;
                }
            }
        }
        else {
            if (currentSegmentIndex > 0) {
                currentSegmentIndex--;
            }
            else {
                if (Settings.Loop) {
                    currentSegmentIndex = Settings.Positions.Length - 1;
                }
                else {
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
        if (Settings == null || Settings.Positions == null || Settings.Positions.Length < 2) return;

        _offset = Application.isPlaying ? _startPosition : transform.position;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < Settings.Positions.Length; i++) {
            Gizmos.DrawWireSphere(Settings.Positions[i] + _offset, 0.2f);
            if (i > 0) {
                Gizmos.DrawLine(Settings.Positions[i - 1] + _offset, Settings.Positions[i] + _offset);
                if (Settings.Loop && Settings.EnableCycle) {
                    Gizmos.DrawLine(Settings.Positions[Settings.Positions.Length - 1] + _offset, Settings.Positions[0] + _offset);
                }
            }
        }
    }
}
