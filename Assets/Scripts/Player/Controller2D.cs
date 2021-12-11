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
        public bool SlidingMaxSlope;
        public Vector2 SlopeNormal;
        public bool DescendigSlope;
        public int FaceDir;
        public Vector2 VelocityOld;

        public void Reset()
        {
            Above = Below = Left = Right = false;
            ClimbingSlope = false;
            DescendigSlope = false;
            SlopeAngleOld = SlopeAngle;
            SlopeAngle = 0;
            SlidingMaxSlope = false;
            SlopeNormal = Vector2.zero;
        }
    }

    [SerializeField] private float _maxSlopeAngle = 75;

    protected override void Start()
    {
        base.Start();
        Collisions.FaceDir = 1;
    }

    public CollisionInfo Collisions;
    
    public void Move(Vector2 deltaMove, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        Collisions.Reset();
        Collisions.VelocityOld = deltaMove;

        if (deltaMove.y < 0)
        {
            DescendSlope(ref deltaMove);
        }

        if (deltaMove.x != 0)
        {
            Collisions.FaceDir = (int)Mathf.Sign(deltaMove.x);
        }

        HorizontalCollisions(ref deltaMove);
        
        if(deltaMove.y != 0)
            VerticalCollisions(ref deltaMove);

        transform.Translate(deltaMove);

        if (standingOnPlatform)
            Collisions.Below = true;
    }
    
    void HorizontalCollisions(ref Vector2 deltaMove)
    {
        float directionX = Collisions.FaceDir;
        float rayLength = Mathf.Abs(deltaMove.x) + SkinWidth;

        if (Mathf.Abs(deltaMove.x) < SkinWidth)
            rayLength = 2 * SkinWidth;

        for (int i = 0; i < _horizontalRaysCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (_horizontalRaySpacing * i);

            var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _collisionLayers);

            if (hit)
            {
                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= _maxSlopeAngle)
                {
                    if (Collisions.DescendigSlope)
                    {
                        Collisions.DescendigSlope = false;
                        deltaMove = Collisions.VelocityOld;
                    }

                    float distanceToSlopeStart = 0;
                    if (slopeAngle != Collisions.SlopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - SkinWidth;
                        deltaMove.x -= distanceToSlopeStart * directionX;
                    }

                    ClimbSlope(ref deltaMove, slopeAngle, hit.normal);
                    deltaMove.x += distanceToSlopeStart * directionX;
                }

                if (!Collisions.ClimbingSlope || slopeAngle > _maxSlopeAngle)
                {
                    deltaMove.x = (hit.distance - SkinWidth) * directionX;
                    rayLength = hit.distance;

                    if (Collisions.ClimbingSlope)
                        deltaMove.y = Mathf.Tan(Collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(deltaMove.x);

                    Collisions.Left = directionX == -1;
                    Collisions.Right = directionX == 1;
                }
            }
            
            Debug.DrawRay(rayOrigin, Vector2.right * (directionX * rayLength), Color.red);
        }
    }
    
    void VerticalCollisions(ref Vector2 deltaMove)
    {
        float directionY = Mathf.Sign(deltaMove.y);
        float rayLength = Mathf.Abs(deltaMove.y) + SkinWidth;
        
        for (int i = 0; i < _verticalRaysCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * i + deltaMove.x);

            var hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, _collisionLayers);

            if (hit)
            {
                deltaMove.y = (hit.distance - SkinWidth) * directionY;
                rayLength = hit.distance;

                if (Collisions.ClimbingSlope)
                {
                    deltaMove.x = deltaMove.y / Mathf.Tan(Collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(deltaMove.x);
                }

                Collisions.Below = directionY == -1;
                Collisions.Above = directionY == 1;
            }

            Debug.DrawRay(rayOrigin, Vector2.down * (directionY * rayLength), Color.red);
        }

        if (Collisions.ClimbingSlope)
        {
            float directionX = Mathf.Sign(deltaMove.x);
            rayLength = Mathf.Abs(deltaMove.x) + SkinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight) +
                                Vector2.up * deltaMove.y;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _collisionLayers);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != Collisions.SlopeAngle)
                {
                    deltaMove.x = (hit.distance - SkinWidth) * directionX;
                    Collisions.SlopeAngle = slopeAngle;
                    Collisions.SlopeNormal = hit.normal;
                }
            }
        }
    }
    
    private void ClimbSlope(ref Vector2 deltaMove, float slopeAngle, Vector2 slopeNormal)
    {
        var moveDistance = Mathf.Abs(deltaMove.x);
        var climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        
        if (!(deltaMove.y <= climbVelocityY)) return;
        
        deltaMove.y = climbVelocityY;
        deltaMove.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(deltaMove.x);
        
        Collisions.Below = true;
        Collisions.ClimbingSlope = true;
        Collisions.SlopeAngle = slopeAngle;
        Collisions.SlopeNormal = slopeNormal;
    }
    
    private void DescendSlope(ref Vector2 deltaMove)
    {
        var maxSlopeHitLeft = Physics2D.Raycast(_raycastOrigins.bottomLeft, Vector2.down,
            Mathf.Abs(deltaMove.y) + SkinWidth, _collisionLayers);
        var maxSlopeHitRight = Physics2D.Raycast(_raycastOrigins.bottomRight, Vector2.down,
            Mathf.Abs(deltaMove.y) + SkinWidth, _collisionLayers);

        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref deltaMove);
            SlideDownMaxSlope(maxSlopeHitRight, ref deltaMove);
        }

        if(Collisions.SlidingMaxSlope) return;
        
        var directionX = Mathf.Sign(deltaMove.x);
        var rayOrigin = (directionX == -1) ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;
        var hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, _collisionLayers);

        if (!hit) return;
        
        var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        
        if (slopeAngle == 0 || !(slopeAngle <= _maxSlopeAngle)) return;
        if (Mathf.Sign(hit.normal.x) != directionX) return;
        
        if (!(hit.distance - SkinWidth <=
              Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(deltaMove.x))) return;
        
        var moveDistance = Mathf.Abs(deltaMove.x);
        var descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        
        deltaMove.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(deltaMove.x);
        deltaMove.y -= descendVelocityY;

        Collisions.SlopeAngle = slopeAngle;
        Collisions.DescendigSlope = true;
        Collisions.Below = true;
        Collisions.SlopeNormal = hit.normal;
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 deltaMove)
    {
        if (!hit) return;
        
        var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        
        if (slopeAngle > _maxSlopeAngle)
        {
            deltaMove.x = hit.normal.x * (Mathf.Abs(deltaMove.y) - hit.distance / Mathf.Tan(slopeAngle * Mathf.Deg2Rad));

            Collisions.SlopeAngle = slopeAngle;
            Collisions.SlidingMaxSlope = true;
            Collisions.SlopeNormal = hit.normal;
            Collisions.Below = true;
        }
    }
}
