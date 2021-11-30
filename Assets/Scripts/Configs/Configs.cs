using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Configs", menuName = "Configs/Configs")]
public class Configs : ScriptableObject {
    public LocationsConfig LocationsConfig;
}