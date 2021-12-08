using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformController : RaycastController
{
    struct PassengerMovement
    {
        public Transform Transform;
        public Vector2 Velocity;
        public bool StandingOnPlatform;
        public bool MovePassangerBeforePlatform;

        public PassengerMovement(Transform transform, Vector2 velocity, bool standingOnPlatform,
            bool movePassangerBeforePlatform)
        {
            Transform = transform;
            Velocity = velocity;
            StandingOnPlatform = standingOnPlatform;
            MovePassangerBeforePlatform = movePassangerBeforePlatform;
        }
    }
    
    [SerializeField] LayerMask _passengerMask;
    [SerializeField] float _speed;
    [SerializeField]private float _waitTime;
    [SerializeField] private bool _cyclic;
    [SerializeField] private AnimationCurve _movementCurve;
    [SerializeField] private Vector3[] _wayPoints;

    private List<PassengerMovement> _passengerMovements;
    Dictionary<Transform, Controller2D> _passengerDictionary = new Dictionary<Transform, Controller2D>();
    
    private Vector3[] _globalWayPoints;
    private int fromWayPointIndex;
    private float percentBetweenWayPoints;
    private float _nextMoveTime;

    protected override void Start()
    {
        base.Start();
        
        _globalWayPoints = new Vector3[_wayPoints.Length];
        
        for (int i = 0; i < _wayPoints.Length; i++)
            _globalWayPoints[i] = transform.TransformPoint(_wayPoints[i]);
    }

    private void Update()
    {
        UpdateRaycastOrigins();
        
        var velocity = CalculatePlatformMovement();
        
        CalculatePassengersMovement(velocity);
        
        MovePassangers(true);
        
        transform.Translate(velocity);
        
        MovePassangers(false);
    }

    Vector3 CalculatePlatformMovement()
    {
        if (Time.time < _nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWayPointIndex %= _globalWayPoints.Length;

        var toWayPointIndex = (fromWayPointIndex + 1) % _globalWayPoints.Length;
        var distanceBetweenWaypoints =
            Vector3.Distance(_globalWayPoints[fromWayPointIndex], _globalWayPoints[toWayPointIndex]);

        percentBetweenWayPoints += Time.deltaTime * _speed / distanceBetweenWaypoints;

        var newPos = Vector3.Lerp(_globalWayPoints[fromWayPointIndex], _globalWayPoints[toWayPointIndex],
            _movementCurve.Evaluate(percentBetweenWayPoints));

        if (percentBetweenWayPoints >= 1)
        {
            percentBetweenWayPoints = 0;
            fromWayPointIndex++;

            if (!_cyclic)
            {
                if (fromWayPointIndex >= _globalWayPoints.Length - 1)
                {
                    fromWayPointIndex = 0;
                    Array.Reverse(_globalWayPoints);
                }
            }

            _nextMoveTime = Time.time + _waitTime;
        }

        return newPos - transform.position;
    }

    void MovePassangers(bool beforeMovePlatform)
    {
        foreach (var passenger in _passengerMovements)
        {
            if (!_passengerDictionary.ContainsKey(passenger.Transform))
            {
                var controller = passenger.Transform.GetComponent<Controller2D>();
                
                if (controller != null)
                    _passengerDictionary.Add(passenger.Transform, controller);
            }

            if (passenger.MovePassangerBeforePlatform == beforeMovePlatform && _passengerDictionary.ContainsKey(passenger.Transform))
                _passengerDictionary[passenger.Transform].Move(passenger.Velocity, passenger.StandingOnPlatform);
        }
    }

    void CalculatePassengersMovement(Vector3 velocity)
    {
        var movedPassengers = new HashSet<Transform>();
        _passengerMovements = new List<PassengerMovement>();
        
        var directionX = Mathf.Sign(velocity.x);
        var directionY = Mathf.Sign(velocity.y);

        if (velocity.y != 0) //Vertical movement
        {
            var rayLength = Mathf.Abs(velocity.y) + SkinWidth;

            for (var i = 0; i < _verticalRaysCount; i++)
            {
                var rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * i);

                var hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, _passengerMask);

                if (!hit) continue;
                if (movedPassengers.Contains(hit.transform)) continue;

                var pushX = (directionY == 1) ? velocity.x : 0;
                var pushY = velocity.y - (hit.distance - SkinWidth) * directionY;


                _passengerMovements.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), directionY > 0,
                    true));
                
                movedPassengers.Add(hit.transform);
            }
        }

        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + SkinWidth;

            for (int i = 0; i < _horizontalRaysCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (_horizontalRaySpacing * i);

                var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _passengerMask);

                if (!hit) continue;
                if (movedPassengers.Contains(hit.transform)) continue;

                var pushX =  velocity.x - (hit.distance - SkinWidth) * directionX;
                var pushY = -SkinWidth;

                // hit.transform.Translate(new Vector3(pushX, pushY));
                
                _passengerMovements.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), false,
                    true));

                movedPassengers.Add(hit.transform);
            }
        }

        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            var rayLength = Mathf.Abs(velocity.y) + SkinWidth*2;

            for (var i = 0; i < _verticalRaysCount; i++)
            {
                var rayOrigin = _raycastOrigins.topLeft + Vector2.right * (_verticalRaySpacing * i);;

                var hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, _passengerMask);

                if (!hit) continue;
                if (movedPassengers.Contains(hit.transform)) continue;

                var pushX = velocity.x;
                var pushY = velocity.y;
                
                _passengerMovements.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), true,
                    false));

                movedPassengers.Add(hit.transform);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_wayPoints != null)
        {
            Gizmos.color = Color.green;
            float size = 0.3f;

            var points = Application.isPlaying
                ? _globalWayPoints
                : _wayPoints.Select(p => transform.TransformPoint(p)).ToArray();

            for (int i = 0; i < points.Length; i++)
            {
                Gizmos.DrawLine(points[i] - Vector3.up * size, points[i] + Vector3.up * size);
                Gizmos.DrawLine(points[i] - Vector3.right * size, points[i] + Vector3.right * size);
            }
        }
    }
}
