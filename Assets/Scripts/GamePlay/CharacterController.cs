using System.Collections;
using System.Collections.Generic;
using PathCreation.Utility;
using UnityEngine;

public class CharacterController : MonoBehaviour, ILightInteractive
{
    [System.Serializable]
    public class Settings
    {
        public int Lives;
        public int Speed;
        public float JumpHeight;
        public float JumpDistance;
    }

    [SerializeField] private Settings _settings;

    private int _currentLives;
    private Vector3 _velocity;
    private bool _isInLite;
    private Vector3 _lightDir;
    private LightController _currentController;
    
    void Start()
    {
        _currentLives = _settings.Lives;
    }

    void StartRun()
    {
        
    }

    public void OnLightOn(string lightID, LightController lightController)
    {
        if(_isInLite) return;
        
        _isInLite = true;
        _currentController = lightController;
    }

    public void OnLightOut(string lightID)
    {
        _isInLite = false;
        _currentController = null;
    }

    void Update()
    {
        if (!_isInLite)
        {
            _velocity += Vector3.down * (9.8f * Time.deltaTime);
            transform.position += _velocity * Time.deltaTime;
        }
        else
        {
            UpdatePointsData();
            
            var currentTime = GetClosestTimeOnPath(transform.position);
            var length = _length;
            
            currentTime += (_settings.Speed * Time.deltaTime) / length;

            var nextPos = GetPointAtTime(currentTime);
            
            transform.position = nextPos;
        }
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
}
