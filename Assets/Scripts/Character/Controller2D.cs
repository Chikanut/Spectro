using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Controller2D : MonoBehaviour
{
    public struct CollisionInfo
    {
        public bool Above, Below;
        public bool Left, Right;

        public void Reset()
        {
            Above = Below = Left = Right = false;
        }
    }
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    private const float SkinWidth = 0.015f;

    [SerializeField] private LayerMask _collisionLayers;
    
    [SerializeField] private int _horizontalRaysCount = 4;
    [SerializeField] private int _verticalRaysCount = 4;

    public CollisionInfo Collisions;
    
    private float _horizontalRaySpacing, _verticalRaySpacing;

    private Collider2D _collider;
    private RaycastOrigins _raycastOrigins;

    void Start()
    {
        _collider = GetComponent<Collider2D>();
        CalculateRaySpacing();
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        Collisions.Reset();

        if(velocity.x != 0)
            HorizontalCollisions(ref velocity);
        
        if(velocity.y != 0)
            VerticalCollisions(ref velocity);

        transform.Translate(velocity);
    }
    
    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + SkinWidth;

        for (int i = 0; i < _horizontalRaysCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (_horizontalRaySpacing * i);

            var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _collisionLayers);

            if (hit)
            {
                velocity.x = (hit.distance - SkinWidth) * directionX;
                rayLength = hit.distance;

                Collisions.Left = directionX == -1;
                Collisions.Right = directionX == 1;
            }
            
            Debug.DrawRay(rayOrigin, Vector3.right * (directionX * rayLength), Color.red);
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + SkinWidth;
        
        for (int i = 0; i < _verticalRaysCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);

            var hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, _collisionLayers);

            if (hit)
            {
                velocity.y = (hit.distance - SkinWidth) * directionY;
                rayLength = hit.distance;
                
                Collisions.Below = directionY == -1;
                Collisions.Above = directionY == 1;
            }

            Debug.DrawRay(rayOrigin, Vector3.down * (directionY * rayLength), Color.red);
        }
    }

    void UpdateRaycastOrigins()
    {
        var bounds = _collider.bounds;
        bounds.Expand(SkinWidth * -2);

        _raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        _raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        _raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        _raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        var bounds = _collider.bounds;
        bounds.Expand(SkinWidth * -2);

        _horizontalRaysCount = Mathf.Clamp(_horizontalRaysCount, 2, int.MaxValue);
        _verticalRaysCount = Mathf.Clamp(_verticalRaysCount, 2, int.MaxValue);

        _horizontalRaySpacing = bounds.size.y / (_horizontalRaysCount - 1);
        _verticalRaySpacing = bounds.size.x / (_verticalRaysCount - 1);
    }

}
