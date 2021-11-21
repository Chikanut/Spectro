using System.Collections.Generic;
using PathCreation;
using PathCreation.Examples;
using UnityEngine;
using UnityEngine.EventSystems;

public class RailMovement : PoolObject
{
    [System.Serializable]
    public class Settings : PoolObjectInfo
    {
        [Range(0, 1)] public float StartPos = 0;
        public bool AutoMovement;
        public float MovementSpeed;
        public EndOfPathInstruction MovementType = EndOfPathInstruction.Reverse;
        public float MovementDumping;
        [HideInInspector] public BezierPath PathData;
    }

    [Header("Components")]
    [SerializeField] public ObjectBase _targetObject;
    [SerializeField] private PathCreator _path;
    [SerializeField] private PathPlacer _placer;

    [Header("Settings")]
    [SerializeField] Settings _settings;

    private float _currentProgress;
    private float _progressChangingVelocity;
    private Vector3 _targetPos;
    private bool _objectInited = false;
    
    private void Start()
    {
        _targetPos = _path.path.GetPointAtTime(_settings.StartPos,_settings.MovementType);
        _currentProgress = _settings.StartPos;
    }

    void Update()
    {
        if(_targetObject == null)
            return;
        else if(!_objectInited)
        {
            InitObject();
        }
        
        if (_settings.AutoMovement)
        {
            _currentProgress += Time.deltaTime * _settings.MovementSpeed;
            _targetObject.transform.position = _path.path.GetPointAtTime(_currentProgress, _settings.MovementType);
        }
        else
        {
            _currentProgress = Mathf.SmoothDamp(_currentProgress, _path.path.GetClosestTimeOnPath(_targetPos),
                ref _progressChangingVelocity, _settings.MovementDumping * Time.deltaTime);
            _targetObject.transform.position = _path.path.GetPointAtTime(_currentProgress, EndOfPathInstruction.Stop);
        }
    }

    void InitObject()
    {
        _targetObject.OnDragAction += OnDrag;
        _objectInited = true;
    }

    void OnDrag(PointerEventData eventData)
    {
        _targetPos = Camera.main.ScreenToWorldPoint(eventData.position);
    }

    public override string SerializeSettings()
    {
        GetDefaultInfo(_settings);
        _settings.Links.Add(_targetObject.InstanceID);
        _settings.PathData = _path.bezierPath;

        return Helpers.XMLHelper.Serialize(_settings);
    }

    public override void AcceptSettings(string info)
    {
        _settings = Helpers.XMLHelper.Deserialize<Settings>(info);
        _path.bezierPath = _settings.PathData;
        _placer.Generate();

        AcceptTransformInfo(_settings);
    }
    
    public override void AcceptObjectsLinks(List<PoolObject> objects)
    {
        if (objects.Count <= 0) return;
        
        var target = (ObjectBase)objects[0];
        if (target != null)
        {
            _targetObject = target;
        }
    }

    public override void ResetState()
    {
        
    }
}
