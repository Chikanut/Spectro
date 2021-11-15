using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectsPool : MonoBehaviour
{
    private static ObjectsPool _instance;

    public static ObjectsPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ObjectsPool>();
                
                if(_instance == null)
                    _instance = new GameObject("_ObjectsPool").AddComponent<ObjectsPool>();

                _instance._objectsParent = _instance.transform;
            }

            return _instance;
        }
        set => _instance = value;
    }

    private Transform _objectsParent;

    private readonly Dictionary<string, List<PoolObject>> _instantiatedObjectsPerType =
        new Dictionary<string, List<PoolObject>>();

    // public ObjectsPool(Transform objectsParent)
    // {
    //     _objectsParent = objectsParent;
    //     _instantiatedObjectsPerType = new Dictionary<string, List<PoolObject>>();
    // }
    
    /// <summary>
    /// Returns existed inactive object of base type.
    /// Instantiates object first if there are no inactive objects of given type yet
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private PoolObject GetObjectOfType(string ObjName, Transform parent, bool isUnique) {
        PoolObject reusableObject;
        if (_instantiatedObjectsPerType.ContainsKey(ObjName)) {
            var objectsList = _instantiatedObjectsPerType[ObjName];
            reusableObject = isUnique ? objectsList.FirstOrDefault() : 
                objectsList.FirstOrDefault(item => item != null && !item.IsActive);
            //if there are no required objects now
            if (reusableObject == null) {
                reusableObject = InstantiateObjectOfType(ObjName, parent);
                objectsList.Add(reusableObject);
            }
            else if(parent) {
                reusableObject.transform.SetParent(parent);
            }
        }
        else {
            reusableObject = InstantiateObjectOfType(ObjName, parent);
            _instantiatedObjectsPerType[ObjName] = new List<PoolObject>{reusableObject};
        }

        reusableObject.IsActive = true;
        reusableObject.ResetState();
        return reusableObject;
    }

    /// <summary>
    /// Returns unique existed object of requested type
    /// Instantiates object first if there are no unique objects of requested type yet
    /// Does not instantiate more than one object
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parent"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetUniqueObjectOfType<T> (string ObjName, Transform parent = null) where T: PoolObject {
        return (T) GetObjectOfType(ObjName, parent, true);
    }
    
    /// <summary>
    /// Returns existed inactive object of requested type.
    /// Instantiates object first if there are no inactive objects of given requested yet
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public T GetObjectOfType<T>(string ObjName, Transform parent = null) where T: PoolObject {
        return (T)GetObjectOfType(ObjName, parent, false);
    }
    
    
    /// <summary>
    /// Adds list of objects of requested type,
    /// adds new objects to pool if count is greater than reusable objects count of given type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public void GetObjectsOfType(string ObjName, int count) {
        List<PoolObject> objectsList;
        if (_instantiatedObjectsPerType.ContainsKey(ObjName)) {
            objectsList = _instantiatedObjectsPerType[ObjName];
        }
        else {
            objectsList = new List<PoolObject>();
            _instantiatedObjectsPerType[ObjName] = objectsList;
        }
        //if we request more objects of given type that exist in pool at this moment
        var reusableObjects = objectsList.Where(item => !item.IsActive).ToList();
        var reusableObjToActivate = Mathf.Min(count, reusableObjects.Count);
        for (int i = 0; i < reusableObjToActivate; i++) {
            reusableObjects[i].IsActive = true;
            count--;
        }
        for (int i = 0; i < count; i++) {
            objectsList.Add(InstantiateObjectOfType(ObjName, _objectsParent));
        }
    }
    
    
    /// <summary>
    /// For each given object type adds given count of objects to pool
    /// </summary>
    /// <param name="objectsPerType"></param>
    public void GetObjectsSet(Dictionary<string, int> objectsPerType, Transform parent = null) {
        foreach (var data in objectsPerType) {
            if (data.Value == 1) {
                GetObjectOfType(data.Key, parent, false);
            }
            else {
                GetObjectsOfType(data.Key, data.Value);
            }
        }
    }
      
    /// <summary>
    /// Returns list of all objects of given type, active and inactive
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public List<PoolObject> GetAllObjectsOfType(string ObjName) {
        if (HasObjectsOfType(ObjName)) {
            return _instantiatedObjectsPerType[ObjName];
        }
        Debug.LogWarning($"There are no objects of type '{ObjName}' yet, returning an empty list");
        return new List<PoolObject>();
    }
    
    /// <summary>
    /// Returns list of all objects of given type that correspond to given state
    /// </summary>
    /// <param name="type"></param>
    /// <param name="inActiveState"></param>
    /// <returns></returns>
    public List<PoolObject> GetAllObjectsOfType(string ObjName, bool inActiveState) {
        if (HasObjectsOfType(ObjName)) {
            return _instantiatedObjectsPerType[ObjName].Where(item => item.IsActive == inActiveState).ToList();
        }
        Debug.LogWarning($"There are no objects of type '{ObjName}' yet, returning an empty list");
        return new List<PoolObject>();
    }

    /// <summary>
    /// Returns dictionary with keys of requested objects type and values of objects list
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public Dictionary<string, List<PoolObject>> GetObjectsSetOfTypes(List<string> types) {
        Dictionary<string, List<PoolObject>> objectsOfType = new Dictionary<string, List<PoolObject>>();
        foreach (var type in types) {
            objectsOfType.Add(type, GetAllObjectsOfType(type));
        }
        return objectsOfType;
    }
    
    /// <summary>
    /// Returns dictionary with keys of requested objects type and values of objects list
    /// where objects correspond to given state
    /// </summary>
    /// <param name="types"></param>
    /// <param name="inActiveState"></param>
    /// <returns></returns>
    public Dictionary<string, List<PoolObject>> GetObjectsSetOfTypes(List<string> types, bool inActiveState) {
        Dictionary<string, List<PoolObject>> objectsOfType = new Dictionary<string, List<PoolObject>>();
        foreach (var type in types) {
            objectsOfType.Add(type, GetAllObjectsOfType(type, inActiveState));
        }
        return objectsOfType;
    }
    
    /// <summary>
    /// Indicates if this objects pool contains any objects of given type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool HasObjectsOfType(string type) {
        return _instantiatedObjectsPerType.ContainsKey(type);
    }
    
    /// <summary>
    /// Instantiates object of given type
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    private PoolObject InstantiateObjectOfType(string name, Transform parent) {
        return ((GameObject)Object.Instantiate(Resources.Load(name), parent ? parent : _objectsParent, false)).GetComponent<PoolObject>();
    }
    
    /// <summary>
    /// Returns dictionary of all objects per type, active and inactive
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, List<PoolObject>> GetAllCurrentObjectsPerType() {
        return _instantiatedObjectsPerType;
    }
    
    /// <summary>
    /// Returns dictionary of all objects per type, objects are filtered by their active state
    /// </summary>
    /// <param name="inActiveState"></param>
    /// <returns></returns>
    public Dictionary<string, List<PoolObject>> GetAllCurrentObjectsPerType(bool inActiveState) {
        return GetObjectsSetOfTypes(_instantiatedObjectsPerType.Keys.ToList(), inActiveState);
    }
    
    /// <summary>
    /// Deactivates all current active objects
    /// </summary>
    public void DeactivateAllObjects() {
        foreach (var data in GetAllCurrentObjectsPerType(true)) {
            foreach (var poolObject in data.Value) {
                poolObject.Destroy();
            }
        }
    }
    
}
