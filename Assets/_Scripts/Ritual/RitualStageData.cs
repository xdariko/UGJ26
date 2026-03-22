using UnityEngine;

[System.Serializable]
public class RitualStageRequirement
{
    public EtherType etherType;
    public int requiredAmount;
}

[System.Serializable]
public class RitualStageData
{
    public string title;
    public RitualStageRequirement[] requirements;
    public GameObject[] objectsToActivate;
}

