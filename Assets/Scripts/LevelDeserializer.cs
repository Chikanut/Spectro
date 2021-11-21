using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class LevelDeserializer : MonoBehaviour
{
    [SerializeField] private TextAsset _levelInfo;

    public void Awake()
    {
        DeserizlizeLevel(_levelInfo.text);   
    }

    Dictionary<string, GameObject> _objectsInstances = new Dictionary<string, GameObject>();
    Dictionary<string, PoolObject> _poolObjects = new Dictionary<string, PoolObject>();
    
    void DeserizlizeLevel(string info)
    {
        var settings = XMLHelper.Deserialize<List<string>>(info);

        foreach (var setting in settings)
            SpawnObject(setting);

        InitLinks();
    }

    void SpawnObject(string info)
    {
        Debug.Log(info);
        var setting = XMLHelper.Deserialize<PoolObjectTransform.PoolObjectInfo>(info);

        var itemIndexSplited = setting.InstanceID.Split('_');

        var objectIndex = string.Join("_", itemIndexSplited[0], itemIndexSplited[1]);
        var componentIndex = int.Parse(itemIndexSplited[2]);

        GameObject targetObject;
            
        if (_objectsInstances.ContainsKey(objectIndex))
        {
            targetObject = _objectsInstances[objectIndex];
        }
        else
        {
            Debug.Log(setting.ObjectName);
            targetObject = ObjectsPool.Instance.GetObjectOfType<PoolObject>(setting.ObjectName).gameObject;
            _objectsInstances.Add(objectIndex, targetObject);
        }

        var obj = targetObject.GetComponents<PoolObject>()[componentIndex];

        obj.AcceptSettings(info);

        _poolObjects.Add(setting.InstanceID, obj);
    }

    void InitLinks()
    {
        foreach (var poolObject in _poolObjects)
        {
            var linkObjects = new List<PoolObject>();
            
            for (int i = 0; i < poolObject.Value.Links.Count; i++)
            {
                if (_poolObjects.ContainsKey(poolObject.Value.Links[i]))
                {
                    linkObjects.Add(_poolObjects[poolObject.Value.Links[i]]);
                }
            }
            
            poolObject.Value.AcceptObjectsLinks(linkObjects);
        }
    }
}
