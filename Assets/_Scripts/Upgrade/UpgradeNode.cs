using UnityEngine;

[System.Serializable]
public class UpgradeNode
{
    public string id;
    public string title;
    public string description;

    public bool unlockedByDefault = false;

    public string[] requiredNodeIds;

    public int maxLevel = 1;
    [HideInInspector] public int currentLevel = 0;

    public UpgradeLevelData[] levels;

    public int MaxLevel => levels != null ? levels.Length : 0;
}
