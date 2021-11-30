using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LocationConfig
{
    public string LocationType;
    public LevelsConfig PatternsInfo;
}

[CreateAssetMenu(fileName = "LocationsConfig", menuName = "Configs/LocationsConfig")]
public class LocationsConfig : ScriptableObject
{
    public List<LocationConfig> LocationConfigs = new List<LocationConfig>();

    public LocationConfig GetNextConfig(string currentLocation)
    {
        for (int i = 0; i < LocationConfigs.Count; i++)
        {
            if (LocationConfigs[i].LocationType == currentLocation && i + 1 < LocationConfigs.Count)
            {
                currentLocation = LocationConfigs[i + 1].LocationType;
                break;
            }
        }
        
        return GetConfig(currentLocation);
    }

    public int GetLocationNum(string loc)
    {
        for (int i = 0; i < LocationConfigs.Count; i++)
        {
            if (LocationConfigs[i].LocationType == loc)
                return i;
        }

        return 0;
    }

    public LocationConfig GetConfig(string type)
    {
        return LocationConfigs.Find(l => l.LocationType == type);
    }
}
