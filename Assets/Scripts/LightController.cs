using System.Collections.Generic;
using System.Linq;
using UnityEngine;

interface ILightInteractive
{
    void OnLightOn(string lightID);
    void OnLightOut(string lightID);
}

public class LightController : MonoBehaviour
{
    [Header("ID")]
    public string LightID = "0";
    
    [Header("Components")]
    [SerializeField] private LineRenderer _line;

    [Header("Settings")]
    [SerializeField] private Vector2 _initialDirection = new Vector2(1, 0); 
    [SerializeField] private float _lineWidth;
    [SerializeField] private LayerMask _reflectLayers;
    [SerializeField] private int _maxReflects = 100;
    [SerializeField] private float _outDistance;

    private List<ILightInteractive> _interactives = new List<ILightInteractive>();
    
    void Start()
    {
        _line.endWidth = _line.startWidth = _lineWidth;
    }

    void Update()
    {
        UpdateLight();
    }

    void UpdateLight()
    {
        var points = new List<Vector3>();
        var prevDir = transform.TransformDirection(_initialDirection);
        var prevPoint = transform.position;
        points.Add(transform.position);
        
        var interactives = new List<ILightInteractive>();
        
        for (int i = 0; i < _maxReflects; i++)
        {
            var hit = Physics2D.CircleCast(prevPoint, _lineWidth, prevDir);

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

                if (_reflectLayers == (_reflectLayers | (1 << hit.collider.gameObject.layer)))
                {
                    var newPoint = hit.point + hit.normal * _lineWidth;
                    var dir = ((Vector3) newPoint - prevPoint).normalized;
                    points.Add(hit.point);
                    prevPoint = newPoint;
                    prevDir = Vector2.Reflect(dir, hit.normal);
                }
                else
                {
                    var newPoint = hit.point + hit.normal * _lineWidth;
                    prevPoint = newPoint;
                    points.Add(hit.point);
                    break;
                }
            }
            else
            {
                points.Add(points[i] + (Vector3) prevDir * _outDistance);
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
}
