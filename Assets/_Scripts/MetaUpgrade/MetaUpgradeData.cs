using UnityEngine;

[System.Serializable]
public class MetaUpgradeData
{
    public string id;
    public string title;
    [TextArea(2, 5)]
    public string description;
    public Sprite icon;
    public MetaUpgradeEffect[] effects;
}