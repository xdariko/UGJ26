using UnityEngine;

[System.Serializable]
public class UpgradeLevelData
{
    public UpgradeCost[] costs;
    public UpgradeEffect[] effects;
}

[System.Serializable]
public class UpgradeEffect
{
    public UpgradeEffectType effectType;
    public float floatValue;
    public int intValue;
    public string stringValue;
}

[System.Serializable]
public class UpgradeCost
{
    public EtherType etherType;
    public int amount;
}

