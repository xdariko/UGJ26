using UnityEngine;

[System.Serializable]
public class UpgradeNode
{
    public string id;
    public string title;
    public string description;

    public bool unlockedByDefault = false;

    public UpgradeRequirement[] requirements;

    public UpgradeLevelData[] levels;

    public int MaxLevel => levels != null ? levels.Length : 0;
}