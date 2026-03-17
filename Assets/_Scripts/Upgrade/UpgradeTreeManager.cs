using System;
using System.Linq;
using UnityEngine;

public class UpgradeTreeManager : MonoBehaviour
{
    [SerializeField] private UpgradeTreeData treeData;

    private void Awake()
    {
        G.upgradeTreeManager = this;
    }

    public bool CanBuy(string nodeId)
    {
        UpgradeNode node = GetNode(nodeId);
        if (node == null) return false;
        if (node.currentLevel >= node.maxLevel) return false;
        if (!IsUnlocked(node)) return false;

        UpgradeLevelData levelData = node.levels[node.currentLevel];
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
        if(node == null) return false;
        if (!CanBuy(nodeId)) return false;

        UpgradeLevelData levelData = node.levels[node.currentLevel];

        if(levelData != null)
        {
            foreach (var cost in levelData.costs)
            {
                if (!G.SpendEther(cost.etherType, cost.amount)) return false;
            }
        }

        ApplyEffects(levelData.effects);

        node.currentLevel++;
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
            if (requiredNode == null || requiredNode.currentLevel <= 0) return false;
        }
        return true;
    }

    public int GetCurrentLevel(string nodeId)
    {
        UpgradeNode node = GetNode(nodeId);
        return node !=null ? node.currentLevel : 0;
    }

    public bool IsMaxLevel(string nodeId)
    {
        UpgradeNode node = GetNode(nodeId);
        if (node == null) return true;
        return node.currentLevel >= node.MaxLevel;
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
            if (requiredNode == null || requiredNode.currentLevel <= 0) return false;
        }
        return true;
    }

    public int GetMaxLevel(string nodeId)
    {
        UpgradeNode node = GetNode(nodeId);
        if (node == null) return 0;
        return node.maxLevel;
    }
}

