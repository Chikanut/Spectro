using System.Collections.Generic;
using System.Linq;
using PathCreation;
using PathCreation.Utility;
using UnityEngine;

interface ILightInteractive
{
    void OnLightOn(string lightID, LightController light);
    void OnLightOut(string lightID);
}

public class LightController : ObjectBase
{
    [System.Serializable]
    public class Settings : PoolObjectInfo
    {
        public string LightID = "0";
        public Vector2 _initialDirection = new Vector2(1, 0); 
        public float _lineWidth;
        public LayerMask _reflectLayers;
        public LayerMask _stopLayers;
        public int _maxReflects = 100;
        public float _outDistance;
    }
    
    
    [Header("Components")]
    [SerializeField] private LineRenderer _line;
    [SerializeField] private Transform _litePoint;

    [Space]
    [SerializeField] private Settings _settings;
    
    private List<ILightInteractive> _interactives = new List<ILightInteractive>();
    
    [HideInInspector] public List<Vector3> Points = new List<Vector3>();
    
    void Start()
    {
        _line.useWorldSpace = true;
        _line.endWidth = _line.startWidth = _settings._lineWidth;
    }

    void Update()
    {
        UpdateLight();
    }

    void UpdateLight()
    {
        var points = new List<Vector3>();
        var prevDir = transform.TransformDirection(_settings._initialDirection);
        var prevPoint = _litePoint.position;
        points.Add(prevPoint);

        var interactives = new List<ILightInteractive>();

        var breakReflects = false;
        for (int i = 0; i < _settings._maxReflects; i++)
        {
            var hits = Physics2D.CircleCastAll(prevPoint, _settings._lineWidth, prevDir);

            if (hits.Length <= 0)
            {
                points.Add(points[i] + (Vector3) prevDir * _settings._outDistance);
                break;
            }

            bool isHit = false;

            foreach (var hit in hits)
            {
                var interactive = hit.collider.gameObject.GetComponent<ILightInteractive>();

                if (interactive != null)
                {
                    if (!_interactives.Contains(interactive))
                    {
                        interactive.OnLightOn(_settings.LightID, this);
                        _interactives.Add(interactive);
                    }

                    interactives.Add(interactive);
                }

                if (_settings._reflectLayers ==
                    (_settings._reflectLayers | (1 << hit.collider.gameObject.layer)))
                {
                    var newPoint = hit.point + hit.normal * _settings._lineWidth;
                    var dir = ((Vector3) newPoint - prevPoint).normalized;
                    points.Add(hit.point);
                    prevPoint = newPoint;
                    prevDir = Vector2.Reflect(dir, hit.normal);
                    isHit = true;
                }
                else if (_settings._stopLayers ==
                         (_settings._stopLayers | (1 << hit.collider.gameObject.layer)))
                {
                    var newPoint = hit.point + hit.normal * _settings._lineWidth;
                    prevPoint = newPoint;
                    points.Add(hit.point);
                    breakReflects = true;
                    isHit = true;
                    break;
                }
            }

            if (!isHit)
            {
                points.Add(points[i] + (Vector3) prevDir * _settings._outDistance);
            }

            if (breakReflects)
                break;
        }

        var interactivesToDestroy = new List<ILightInteractive>();

        foreach (var interactive in _interactives.Where(interactive => !interactives.Contains(interactive)))
        {
            interactive.OnLightOut(_settings.LightID);
            interactivesToDestroy.Add(interactive);
        }

        foreach (var destroyInt in interactivesToDestroy)
        {
            _interactives.Remove(destroyInt);
        }

        _line.positionCount = points.Count;
        _line.SetPositions(points.ToArray());

        Points = points;
    }
    
    #region PoolObject

    public override string SerializeSettings()
    {
        GetDefaultInfo(_settings);

        return Helpers.XMLHelper.Serialize(_settings);
    }

    public override void AcceptSettings(string info)
    {
        _settings = Helpers.XMLHelper.Deserialize<Settings>(info);

        AcceptTransformInfo(_settings);
    }

    public override void AcceptObjectsLinks(List<PoolObject> objects)
    {
        
    }

    public override void ResetState()
    {
        
    }
    
    #endregion
}
