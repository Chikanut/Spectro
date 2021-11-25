using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Controller2D : RaycastController
{
    public struct CollisionInfo
    {
        public bool Above, Below;
        public bool Left, Right;
        public bool ClimbingSlope;
        public float SlopeAngle;
        public float SlopeAngleOld;
        public bool DescendigSlope;

        public Vector2 VelocityOld;

        public void Reset()
        {
            Above = Below = Left = Right = false;
            ClimbingSlope = false;
            DescendigSlope = false;
            SlopeAngleOld = SlopeAngle;
            SlopeAngle = 0;
        }
    }

    [SerializeField] private float _maxClimbAngle = 45;
    [SerializeField] private float _maxDescendAngle = 75;

    public CollisionInfo Collisions;
    
    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        Collisions.Reset();
        Collisions.VelocityOld = velocity;

        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

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
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= _maxClimbAngle)
                {
                    if (Collisions.DescendigSlope)
                    {
                        Collisions.DescendigSlope = false;
                        velocity = Collisions.VelocityOld;
                    }

                    float distanceToSlopeStart = 0;
                    if (slopeAngle != Collisions.SlopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - SkinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }

                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (!Collisions.ClimbingSlope || slopeAngle > _maxClimbAngle)
                {
                    velocity.x = (hit.distance - SkinWidth) * directionX;
                    rayLength = hit.distance;

                    if (Collisions.ClimbingSlope)
                        velocity.y = Mathf.Tan(Collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);

                    Collisions.Left = directionX == -1;
                    Collisions.Right = directionX == 1;
                }
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

                if (Collisions.ClimbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(Collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                Collisions.Below = directionY == -1;
                Collisions.Above = directionY == 1;
            }

            Debug.DrawRay(rayOrigin, Vector3.down * (directionY * rayLength), Color.red);
        }

        if (Collisions.ClimbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + SkinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight) +
                                Vector2.up * velocity.y;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _collisionLayers);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != Collisions.SlopeAngle)
                {
                    velocity.x = (hit.distance - SkinWidth) * directionX;
                    Collisions.SlopeAngle = slopeAngle;
                }
            }
        }
    }
    
    private void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        var moveDistance = Mathf.Abs(velocity.x);
        var climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        
        if (!(velocity.y <= climbVelocityY)) return;
        
        velocity.y = climbVelocityY;
        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
        
        Collisions.Below = true;
        Collisions.ClimbingSlope = true;
        Collisions.SlopeAngle = slopeAngle;
    }
    
    private void DescendSlope(ref Vector3 velocity)
    {
        var directionX = Mathf.Sign(velocity.x);
        var rayOrigin = (directionX == -1) ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;
        var hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, _collisionLayers);

        if (!hit) return;
        
        var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        
        if (slopeAngle == 0 || !(slopeAngle <= _maxDescendAngle)) return;
        if (Mathf.Sign(hit.normal.x) != directionX) return;
        
        if (!(hit.distance - SkinWidth <=
              Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))) return;
        
        var moveDistance = Mathf.Abs(velocity.x);
        var descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        
        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
        velocity.y -= descendVelocityY;

        Collisions.SlopeAngle = slopeAngle;
        Collisions.DescendigSlope = true;
        Collisions.Below = true;
    }
}
