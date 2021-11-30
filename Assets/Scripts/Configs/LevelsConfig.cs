using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LevelConfig {
    public TextAsset SerializedLevel;
}

[CreateAssetMenu(fileName = "LevelsConfig", menuName = "Configs/LevelsConfig")]
public sealed class LevelsConfig : ScriptableObject
{
    public List<LevelConfig> LevelConfigs = new List<LevelConfig>();
}