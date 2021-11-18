using System.Collections.Generic;
using System.Linq;
using UnityEngine;

interface ILightInteractive
{
    void OnLightOn(string lightID);
    void OnLightOut(string lightID);
}

public class LightController : PoolObject
{
    [System.Serializable]
    public class Settings : PoolObjectInfo
    {
       
        public Vector2 _initialDirection = new Vector2(1, 0); 
        public float _lineWidth;
        public LayerMask _reflectLayers;
        public int _maxReflects = 100;
        public float _outDistance;
    }

    [Header("ID")]
    public string LightID = "0";
    
    [Header("Components")]
    [SerializeField] private LineRenderer _line;
    [SerializeField] private Transform _litePoint;

    [Header("Settings")]
    [SerializeField] private Settings _settings;
    
    private List<ILightInteractive> _interactives = new List<ILightInteractive>();
    
    void Start()
    {
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
        
        for (int i = 0; i < _settings._maxReflects; i++)
        {
            var hit = Physics2D.CircleCast(prevPoint, _settings._lineWidth, prevDir);

            if (hit.collider != null)
            {
                var interactive = hit.collider.gameObject.GetComponent<ILightInteractive>();

                if (interactive != null)
                {
                    if (!_interactives.Contains(interactive))
                    {
                        interactive.OnLightOn(LightID);
                        _interactives.Add(interactive);
                    }

                    interactives.Add(interactive);
                }

                if (_settings._reflectLayers == (_settings._reflectLayers | (1 << hit.collider.gameObject.layer)))
                {
                    var newPoint = hit.point + hit.normal * _settings._lineWidth;
                    var dir = ((Vector3) newPoint - prevPoint).normalized;
                    points.Add(hit.point);
                    prevPoint = newPoint;
                    prevDir = Vector2.Reflect(dir, hit.normal);
                }
                else
                {
                    var newPoint = hit.point + hit.normal * _settings._lineWidth;
                    prevPoint = newPoint;
                    points.Add(hit.point);
                    break;
                }
            }
            else
            {
                points.Add(points[i] + (Vector3) prevDir * _settings._outDistance);
                break;
            }
        }
        
        var interactivesToDestroy = new List<ILightInteractive>();

        foreach (var interactive in _interactives.Where(interactive => !interactives.Contains(interactive)))
        {
            interactive.OnLightOut(LightID);
            interactivesToDestroy.Add(interactive);
        }

        foreach (var destroyInt in interactivesToDestroy)
        {
            _interactives.Remove(destroyInt);
        }

        _line.positionCount = points.Count;
        _line.SetPositions(points.ToArray());
    }

    #region PoolObject

    public override string SerializeSettings()
    {
        GetDefaultInfo(_settings);

        _settings.SelfDestroy = SelfDestroy;

        return Helpers.XMLHelper.Serialize(_settings);
    }

    public override void AcceptSettings(string info)
    {
        _settings = Helpers.XMLHelper.Deserialize<Settings>(info);

        AcceptTransformInfo(_settings);

        SelfDestroy = _settings.SelfDestroy;
    }

    public override void ResetState()
    {
        
    }
    
    #endregion
}
