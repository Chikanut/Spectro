using System.Collections.Generic;
using Packages.Utils.Extensions;
using UnityEngine;

[System.Serializable]
public abstract class PoolObject : PoolObjectTransform
{
    public virtual string SerializeSettings()
    {
        var infoClass = new PoolObjectInfo();

        GetDefaultInfo(infoClass);
        
        return Helpers.XMLHelper.Serialize(infoClass);
    }

    public virtual void AcceptSettings(string info)
    {
        var infoClass = Helpers.XMLHelper.Deserialize<PoolObjectInfo>(info);

        AcceptTransformInfo(infoClass);
    }

    public abstract void AcceptObjectsLinks(List<PoolObject> objects);
    
    public abstract void ResetState();

    public virtual void Destroy() {
        ResetState();
        
        IsActive = false;
    }
    

    public virtual bool IsActive {
        get => gameObject.activeSelf;
        set => gameObject.SetActive(value);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var bounds = gameObject.GetMaxBounds();
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
