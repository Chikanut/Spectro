using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class LightTriggerListener : MonoBehaviour
{
    public abstract void Activate();
    public abstract void Disactivate();
}

public class LightTrigger : MonoBehaviour, ILightInteractive
{
    [Header("Components")]
    [SerializeField] List<LightTriggerListener> _listeners = new List<LightTriggerListener>();

    [Header("Settings")] [SerializeField] private List<string> _targetIds = new List<string>();
    
    List<string> _lightsIn = new List<string>();
    
    public void OnLightOn(string lightID)
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
        return _targetIds.All(a => _lightsIn.Any(b => b == a));
    }
}
