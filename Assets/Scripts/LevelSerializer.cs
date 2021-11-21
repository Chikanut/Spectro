using System.Collections.Generic;
using System.IO;
using System.Linq;
using Helpers;
using UnityEditor;
using UnityEngine;

public class LevelSerializer : MonoBehaviour
{
    private string _getDirectory = "Assets/_Configs/Levels";

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Serialize();
        }
    }

    private Dictionary<GameObject, string> _savedObjects = new Dictionary<GameObject, string>();

    void Serialize()
    {
        _savedObjects.Clear();
        
        var objects = FindObjectsOfType<PoolObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            if (_savedObjects.ContainsKey(objects[i].gameObject))
            {
                var id = _savedObjects[objects[i].gameObject];
                var splited = id.Split('_');
                
                splited[2] = objects[i].gameObject.GetComponents<PoolObject>().ToList().IndexOf(objects[i]).ToString();
                objects[i].InstanceID = string.Join("_",splited[0], splited[1], splited[2]);
            }
            else
            {
                objects[i].InstanceID = "PoolObject_" + _savedObjects.Count + "_" +
                                        objects[i].gameObject.GetComponents<PoolObject>().ToList().IndexOf(objects[i]);
                _savedObjects.Add(objects[i].gameObject, objects[i].InstanceID);
            }
        }

        var infos = objects.Select(t => t.SerializeSettings()).ToList();
        
        var buildPath = EditorUtility.SaveFilePanel(
            "Serialize level",
            _getDirectory,
            "Level_*",
            "txt"
        );
        string json = XMLHelper.Serialize(infos);
        File.WriteAllText(buildPath, json);
    }
}
