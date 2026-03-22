using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeTreeManager : MonoBehaviour
{
    [SerializeField] private UpgradeTreeData treeData;
    private Dictionary<string, int> nodeLevels = new();

    private void Awake()
    {
        G.upgradeTreeManager = this;
    }

    public bool CanBuy(string nodeId)
    {
        UpgradeNode node = GetNode(nodeId);
        if (node == null || node.levels == null) return false;

        int currentLevel = GetCurrentLevel(nodeId);

        if (currentLevel < 0 || currentLevel >= node.levels.Length) return false;
        if (!IsUnlocked(node)) return false;

        UpgradeLevelData levelData = node.levels[currentLevel];
        if (levelData == null) return false;

        if (levelData.costs != null)
        {
            foreach (var cost in levelData.costs)
            {
                if (G.GetEther(cost.etherType) < cost.amount) return false;
            }
        }

        return true;
    }

    public bool Buy(string nodeId)
    {
        UpgradeNode node = GetNode(nodeId);
        if (node == null) return false;
        if (!CanBuy(nodeId)) return false;

        int currentLevel = GetCurrentLevel(nodeId);
        UpgradeLevelData levelData = node.levels[currentLevel];

        if (levelData != null && levelData.costs != null)
        {
            foreach (var cost in levelData.costs)
            {
                if (!G.SpendEther(cost.etherType, cost.amount)) return false;
            }
        }

        ApplyEffects(levelData.effects);
        nodeLevels[nodeId] = currentLevel + 1;
        return true;
    }

    private void ApplyEffects(UpgradeEffect[] effects)
    {
        if (effects == null) return;

        foreach(var effect in effects)
        {
            switch (effect.effectType)
            {
                case UpgradeEffectType.AddClickDamage:
                    G.ClickDamage += effect.intValue;
                    break;
                case UpgradeEffectType.AddClickRadius:
                    G.ClickRadius += effect.floatValue;
                    break;
                case UpgradeEffectType.AddCritChance:
                    G.CritChance += effect.floatValue;
                    break;
                case UpgradeEffectType.AddCritMultiplier:
                    G.CritMultiplier += effect.floatValue;
                    break;
                case UpgradeEffectType.UnlockEnemy:
                    G.UnlockEnemy(effect.stringValue);
                    break;

            }
        }
    }

    private UpgradeNode GetNode(string nodeId)
    {
        if (treeData == null || treeData.nodes == null) return null;
        return treeData.nodes.FirstOrDefault(n=> n.id == nodeId);
    }

    private bool IsUnlocked(UpgradeNode node)
    {
        if(node.unlockedByDefault) return true;
        if(node.requiredNodeIds == null || node.requiredNodeIds.Length == 0) return true;

        foreach (string requiredId in node.requiredNodeIds)
        {
            UpgradeNode requiredNode = GetNode(requiredId);
            if (requiredNode == null || GetCurrentLevel(requiredId) <= 0) return false;
        }
        return true;
    }

    public int GetCurrentLevel(string nodeId)
    {
        return nodeLevels.TryGetValue(nodeId, out var level) ? level : 0;
    }

    public bool IsMaxLevel(string nodeId)
    {
        UpgradeNode node = GetNode(nodeId);
        if (node == null || node.levels == null) return true;

        return GetCurrentLevel(nodeId) >= node.levels.Length;
    }

    public bool ShouldNodeBeVisible(string nodeId)
    {
        UpgradeNode node = GetNode(nodeId);
        if (node == null) return false;
        if (node.unlockedByDefault) return true;
        if (node.requiredNodeIds == null || node.requiredNodeIds.Length == 0) return true;

        foreach (string requiredId in node.requiredNodeIds)
        {
            UpgradeNode requiredNode = GetNode(requiredId);
            if (requiredNode == null || GetCurrentLevel(requiredId) <= 0) return false;
        }
        return true;
    }

    public int GetMaxLevel(string nodeId)
    {
        UpgradeNode node = GetNode(nodeId);
        if (node == null || node.levels == null) return 0;
        return node.levels.Length;
    }

    public void GetCurrentLevelCosts(string nodeId, out int whiteCost, out int redCost, out int purpleCost)
    {
        whiteCost = 0;
        redCost = 0;
        purpleCost = 0;

        UpgradeNode node = GetNode(nodeId);
        if (node == null) return;

        int currentLevel = GetCurrentLevel(nodeId);
        if (node.levels == null || currentLevel < 0 || currentLevel >= node.levels.Length) return;

        UpgradeLevelData levelData = node.levels[currentLevel];
        if (levelData == null || levelData.costs == null) return;

        foreach (var cost in levelData.costs)
        {
            switch (cost.etherType)
            {
                case EtherType.White: whiteCost += cost.amount; break;
                case EtherType.Red: redCost += cost.amount; break;
                case EtherType.Purple: purpleCost += cost.amount; break;
            }
        }
    }

    public string GetNodeTitle(string nodeId)
    {
        UpgradeNode node = GetNode(nodeId);
        if (node == null) return string.Empty;
        return node.title;
    }

    public string GetNodeDescription(string nodeId)
    {
        UpgradeNode node = GetNode(nodeId);
        if (node == null) return string.Empty;
        return node.description;
    }
}

