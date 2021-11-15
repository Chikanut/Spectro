using PathCreation;
using UnityEngine;
using UnityEngine.EventSystems;

public class RailMovement : MonoBehaviour, IDragHandler
{
    [Header("Components")]
    [SerializeField] private PathCreator _path;

    [Header("Settings")]
    [Range(0,1)]
    [SerializeField] private float _startPos = 0;
    [SerializeField] private bool _autoMovement;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private EndOfPathInstruction _movementType = EndOfPathInstruction.Reverse;
    [SerializeField] private float _movementDumping;

    private float _currentProgress;
    private float _progresChangingVelocity;
    private Vector3 _targetPos;
    
    private void Start()
    {
        _targetPos = _path.path.GetPointAtTime(_startPos, _movementType);
        _currentProgress = _startPos;
    }

    void Update()
    {
        if (_autoMovement)
        {
            _currentProgress += Time.deltaTime * _movementSpeed;
            transform.position = _path.path.GetPointAtTime(_currentProgress, _movementType);
        }
        else
        {
            _currentProgress = Mathf.SmoothDamp(_currentProgress, _path.path.GetClosestTimeOnPath(_targetPos),
                ref _progresChangingVelocity, _movementDumping * Time.deltaTime);
            transform.position = _path.path.GetPointAtTime(_currentProgress, EndOfPathInstruction.Stop);
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        _targetPos = Camera.main.ScreenToWorldPoint(eventData.position);
    }
}
