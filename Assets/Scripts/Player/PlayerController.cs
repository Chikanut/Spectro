using System.Collections;
using System.Collections.Generic;
using PathCreation.Utility;
using ShootCommon.Views.Mediation;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerController : View
{
    [System.Serializable]
    public class Settings
    {
        [Header("Body")]
        public int Lives;
        
        [Header("Movement")]
        public float MovementSpeed;
        public float GroundAccelerationTime = 1;
        
        [Header("Jumping")]
        public float JumpHeight;
        public float TimeToJumpApex = 0.4f;
        public float AirborneAccelerationTIme = 0.2f;
        
        [Header("Wall Jump")]
        public float WallSlideSpeedMax = 3;
        public Vector2 WallJumpClimb;
        public Vector2 WallJumpOff;
        public Vector2 WallLeap;
        public float WallStickTime = 0.24f;
    }
    
    [System.Serializable]
    public class Status
    {
        public int CurrentLives;
        public float JumpVelocity;
        public int WallDirX;
        public bool WallSliding;
    }

    [SerializeField] private Settings _settings;
    [SerializeField] private Status _status;

   
    private bool _isInLite;
    private Vector3 _lightDir;
    private LightController _currentController;

    private Controller2D _controller;
    
    private float _gravity = 20;
    private Vector3 _velocity;
    private float _timeToWallUnstick;
    private float _veloctyXSmoothing;
    
    void Awake()
    {
        _status.CurrentLives = _settings.Lives;
        _controller = GetComponent<Controller2D>();

        _gravity = (2 * _settings.JumpHeight) / Mathf.Pow(_settings.TimeToJumpApex, 2);
        _status.JumpVelocity = _gravity * _settings.TimeToJumpApex;
    }
    
    private Vector2 _directionalInput;
    private bool _jumping;

    public void SetDirectionalInput(Vector2 input)
    {
        _directionalInput = input;
    }

    public void OnJump()
    {
        if (_status.WallSliding)
        {
            if (_status.WallDirX  == _directionalInput.x)
            {
                _velocity.x = -_status.WallDirX  * _settings.WallJumpClimb.x;
                _velocity.y = _settings.WallJumpClimb.y;
            }else if (_directionalInput.x == 0)
            {
                _velocity.x = -_status.WallDirX  * _settings.WallJumpOff.x;
                _velocity.y = _settings.WallJumpOff.y;
            }
            else
            {
                _velocity.x = -_status.WallDirX  * _settings.WallLeap.x;
                _velocity.y = _settings.WallLeap.y;
            }
        }

        if (_controller.Collisions.Below)
        {
            if (_controller.Collisions.SlidingMaxSlope)
            {
                // if (_directionalInput.x != -Mathf.Sign(_controller.Collisions.SlopeNormal.x))//not jumping against slope
                // {
                    _velocity.y = _status.JumpVelocity * _controller.Collisions.SlopeNormal.y;
                    _velocity.x = _status.JumpVelocity * _controller.Collisions.SlopeNormal.x;
                // }
            }else
                _velocity.y = _status.JumpVelocity;
        }
    }

    void Update()
    {
        UpdateMovementInfo();
        UpdateWallMovementInfo();
        
        _controller.Move(_velocity * Time.deltaTime);

        if (_controller.Collisions.Above || _controller.Collisions.Below)
        {
            if (_controller.Collisions.SlidingMaxSlope)
                _velocity.y += _controller.Collisions.SlopeNormal.y * -_gravity * Time.deltaTime;
            else
                _velocity.y = 0;
        }
    }

    void UpdateMovementInfo()
    {
        var targetVelocityX = _directionalInput.x * _settings.MovementSpeed;

        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _veloctyXSmoothing,
            _controller.Collisions.Below ? _settings.GroundAccelerationTime : _settings.AirborneAccelerationTIme);
        _velocity += Vector3.down * (_gravity * Time.deltaTime);
    }

    void UpdateWallMovementInfo()
    {
        _status.WallDirX = (_controller.Collisions.Left) ? -1 : 1;
        _status.WallSliding = false;
        if ((_controller.Collisions.Left || _controller.Collisions.Right) && !_controller.Collisions.Below && _velocity.y < 0)
        {
            _status.WallSliding = true;
            if (_velocity.y < -_settings.WallSlideSpeedMax)
            {
                _velocity.y = -_settings.WallSlideSpeedMax;
            }

            if (_timeToWallUnstick > 0)
            {
                _velocity.x = 0;
                _veloctyXSmoothing = 0;
                
                if ((int) Mathf.Sign(_directionalInput.x) != _status.WallDirX  && _directionalInput.x != 0)
                    _timeToWallUnstick -= Time.deltaTime;
                else
                    _timeToWallUnstick = _settings.WallStickTime;
            }
            else
            {
                _timeToWallUnstick = _settings.WallStickTime;
            }
        }  
    }

    //We gona make this logic in future
    #region LightMovement


    void UpdateLightMovement()
    {
        if (_jumping)
        {
            _velocity.y = _status.JumpVelocity;
            _isInLite = false;
            return;
        }
        
        UpdatePointsData();
        
        var currentTime = GetClosestTimeOnPath(transform.position);
        var length = _length;
        
        currentTime += (_settings.MovementSpeed * Time.deltaTime) / length;
        
        var nextPos = GetPointAtTime(currentTime);
        
        transform.position = nextPos;
    }

    List<Vector3> _points = new List<Vector3>();
    List<float> _times = new List<float>();
    List<float> _cumulativeLengthAtEachVertex = new List<float>();
    float _length;

    void UpdatePointsData()
    {
        _points = _currentController.Points
            ;        _times = new List<float>();
        _cumulativeLengthAtEachVertex = new List<float>();
        _length = 0;
        
        if(_points.Count <= 1) return;
        
        for (int i = 1; i < _points.Count; i++)
        {
            _length += Vector3.Distance(_points[i], _points[i - 1]);
        }

        var currentLength = 0f;
        for (int i = 0; i < _points.Count; i++)
        {
            if(i > 0)
                currentLength += Vector3.Distance(_points[i], _points[i - 1]);
            
            _cumulativeLengthAtEachVertex.Add(_length);
            _times.Add(currentLength/_length);
        }
    }
    
    Vector3 GetClosestPointOnPath (Vector3 worldPoint)
    {
        var data = CalculateClosestPointOnPathData (worldPoint);
        var result = Vector3.Lerp (_points[data.a], _points[data.b], data.t);
        return result;
    }
    
    Vector3 GetPointAtTime (float t) {
        var data = CalculatePercentOnPathData (t);
        return Vector3.Lerp (_points [data.a], _points [data.b], data.t);
    }
    
    float GetClosestTimeOnPath (Vector3 worldPoint)
    {
        var data = CalculateClosestPointOnPathData (worldPoint);
        return Mathf.Lerp (_times[data.a], _times[data.b], data.t);
    }
    
    float GetClosestDistanceAlongPath (Vector3 worldPoint) {
        var data = CalculateClosestPointOnPathData(worldPoint);
        return Mathf.Lerp(_cumulativeLengthAtEachVertex[data.a], _cumulativeLengthAtEachVertex[data.b], data.t);
    }

    (int a, int b, float t ) CalculatePercentOnPathData(float t)
    {
        t = Mathf.Clamp01(t);

        var prevIndex = 0;
        var nextIndex = _times.Count - 1;
        var i = Mathf.RoundToInt(t * (_times.Count - 1));

        while (true)
        {
            if (t <= _times[i])
                nextIndex = i;
            else
                prevIndex = i;

            i = (nextIndex + prevIndex) / 2;

            if (nextIndex - prevIndex <= 1)
                break;
        }

        var abPercent = Mathf.InverseLerp(_times[prevIndex], _times[nextIndex], t);
        return (prevIndex, nextIndex, abPercent);
    }

    (int a, int b, float t ) CalculateClosestPointOnPathData (Vector3 localPoint) {
        var minSqrDst = float.MaxValue;
        var closestPoint = Vector3.zero;
        var closestSegmentIndexA = 0;
        var closestSegmentIndexB = 0;

        for (var i = 0; i < _points.Count; i++) {
            var nextI = i + 1;
            
            if (nextI >= _points.Count)
                break;

            var closestPointOnSegment = MathUtility.ClosestPointOnLineSegment(localPoint, _points[i], _points[nextI]);
            var sqrDst = (localPoint - closestPointOnSegment).sqrMagnitude;
            
            if (!(sqrDst < minSqrDst)) continue;
            
            minSqrDst = sqrDst;
            closestPoint = closestPointOnSegment;
            closestSegmentIndexA = i;
            closestSegmentIndexB = nextI;

        }
        var closestSegmentLength = (_points[closestSegmentIndexA] - _points[closestSegmentIndexB]).magnitude;
        var t = (closestPoint - _points[closestSegmentIndexA]).magnitude / closestSegmentLength;
        
        return (closestSegmentIndexA, closestSegmentIndexB, t);
    }
    #endregion
    
}
