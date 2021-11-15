using UnityEngine;

public class PoolObjectTransform : MonoBehaviour
{
    public class PoolObjectInfo
    {
        public bool SelfDestroy;
        public TweenMovement.MovingSettings Movement;
        public TweenRotation.RotationSettings Rotation;
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

    protected void GetTransformInfo(PoolObjectInfo info)
    {
        var movingObject = GetComponent<TweenMovement>();

        info.Movement = movingObject != null ? movingObject.Settings : new TweenMovement.MovingSettings();

        var rotationObject = GetComponent<TweenRotation>();

        info.Rotation = rotationObject != null ? rotationObject.Settings : new TweenRotation.RotationSettings();
    }

    protected void AcceptTransformInfo(PoolObjectInfo settings)
    {
        Movement.Init(settings.Movement);
        Rotation.Init(settings.Rotation);
    }
}
