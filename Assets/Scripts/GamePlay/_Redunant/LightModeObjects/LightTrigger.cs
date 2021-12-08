using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class LightTriggerListener : ObjectBase
{
    public abstract void Activate();
    public abstract void Disactivate();
}

public class LightTrigger : ObjectBase, ILightInteractive
{
    [System.Serializable]
    public new class Settings : PoolObjectInfo
    {
        public List<string> TargetIDs = new List<string>();
    }
    
    [Header("Components")]
    public bool SerializeLinks;
    [SerializeField] List<LightTriggerListener> _listeners = new List<LightTriggerListener>();

    [SerializeField] private Settings _settings;
    
    List<string> _lightsIn = new List<string>();
    
    public void OnLightOn(string lightID, LightController lightController)
    {
        var isReady = IsLightReady();
        
        _lightsIn.Add(lightID);

        if (isReady || !IsLightReady()) return;
        
        foreach (var listener in _listeners)
            listener.Activate();
    }

    public void OnLightOut(string lightID)
    {
        var isReady = IsLightReady();
        
        if(_lightsIn.Contains(lightID))
            _lightsIn.Remove(lightID);

        if (!isReady || IsLightReady()) return;
        
        foreach (var listener in _listeners)
            listener.Disactivate();
    }

    bool IsLightReady()
    {
        return _settings.TargetIDs.All(a => _lightsIn.Any(b => b == a));
    }
    
    public override string SerializeSettings()
    {
        GetDefaultInfo(_settings);

        if (!SerializeLinks) return Helpers.XMLHelper.Serialize(_settings);
        
        foreach (var listener in _listeners)
            _settings.Links.Add(listener.InstanceID);

        return Helpers.XMLHelper.Serialize(_settings);
    }

    public override void AcceptSettings(string info)
    {
        _settings = Helpers.XMLHelper.Deserialize<Settings>(info);

        AcceptTransformInfo(_settings);
    }

    public override void AcceptObjectsLinks(List<PoolObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            var lightTrigger = (LightTriggerListener) objects[i];
            if(lightTrigger != null)
                _listeners.Add(lightTrigger);
        }

        base.AcceptObjectsLinks(objects);
    }
}
