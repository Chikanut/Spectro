using System.Collections.Generic;
using System.Xml.Serialization;
using Packages.Utils.Extensions;
using UnityEngine;

public class PoolObjectTransform : MonoBehaviour
{
    public string ObjectName;
    [HideInInspector]public string InstanceID;
    [HideInInspector]public List<string> Links;

    [System.Serializable, XmlRoot("Settings")]
    public class PoolObjectInfo
    {
        [HideInInspector] public string ObjectName;
        
        [HideInInspector] public string InstanceID;
        [HideInInspector] public List<string> Links;
        
        [HideInInspector] public Bounds ObjectBounds;
        
        [HideInInspector] public Vector3 LocalPosition;
        [HideInInspector] public Quaternion LocalRotation;
        [HideInInspector] public Vector3 LocalScale;
        
        [HideInInspector] public TweenMovement.MovingSettings Movement;
        [HideInInspector] public TweenRotation.RotationSettings Rotation;
    }

    private TweenMovement _movement;
    public TweenMovement Movement
    {
        get
        {
            if (_movement == null)
                _movement = GetComponent<TweenMovement>();

            if (_movement == null)
                _movement = gameObject.AddComponent<TweenMovement>();

            return _movement;
        }
    }

    private TweenRotation _rotation;
    public TweenRotation Rotation
    {
        get
        {
            if (_rotation == null)
                _rotation = GetComponent<TweenRotation>();

            if (_rotation == null)
                _rotation = gameObject.AddComponent<TweenRotation>();

            return _rotation;
        }
    }

    protected void GetDefaultInfo(PoolObjectInfo info)
    {
        info.ObjectName =  ObjectName;
        info.ObjectBounds = gameObject.GetMaxBounds();
        info.InstanceID = InstanceID;
        var transformBuffer = transform;
        info.LocalPosition = transformBuffer.localPosition;
        info.LocalRotation = transformBuffer.localRotation;
        info.LocalScale = transformBuffer.localScale;
        
        var movingObject = GetComponent<TweenMovement>();

        info.Movement = movingObject != null ? movingObject.Settings : new TweenMovement.MovingSettings();

        var rotationObject = GetComponent<TweenRotation>();

        info.Rotation = rotationObject != null ? rotationObject.Settings : new TweenRotation.RotationSettings();
    }

    protected void AcceptTransformInfo(PoolObjectInfo settings)
    {
        ObjectName = settings.ObjectName;
        InstanceID = settings.InstanceID;
        Links = settings.Links;
        
        var transformBuffer = transform;
        transformBuffer.localPosition = settings.LocalPosition;
        transformBuffer.localRotation =  settings.LocalRotation;
        transformBuffer.localScale = settings.LocalScale;
        
        Movement.Init(settings.Movement);
        Rotation.Init(settings.Rotation);
    }
}
