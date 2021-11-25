using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RaycastController : MonoBehaviour
{
    protected struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
    
    protected const float SkinWidth = 0.015f;
    
    [SerializeField] protected LayerMask _collisionLayers;
    [SerializeField] protected int _horizontalRaysCount = 4;
    [SerializeField] protected int _verticalRaysCount = 4;
    
    protected RaycastOrigins _raycastOrigins;
    protected float _horizontalRaySpacing, _verticalRaySpacing;
    
    private Collider2D _collider;

    protected virtual void Start()
    {
        _collider = GetComponent<Collider2D>();
        CalculateRaySpacing();
    }

    protected void UpdateRaycastOrigins()
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
