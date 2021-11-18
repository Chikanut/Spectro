using System.Linq;
using Helpers;
using UnityEngine;

public class LevelSerializer : MonoBehaviour
{
    string Serialize()
    {
        var objects = FindObjectsOfType<PoolObject>();
        var infos = objects.Select(t => t.SerializeSettings()).ToList();
        return XMLHelper.Serialize(infos);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(Serialize());
        }
    }
}
