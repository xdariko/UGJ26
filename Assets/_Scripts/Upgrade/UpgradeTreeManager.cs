using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeTreeManager : MonoBehaviour
{
    [SerializeField] private UpgradeTreeData treeData;
    private Dictionary<string, int> nodeLevels = new();
    private Dictionary<string, EnemyRuntimeData> enemyRuntime = new Dictionary<string, EnemyRuntimeData>();

    private void Awake()
    {
        G.upgradeTreeManager = this;
        BuildEnemyRuntime();
    }

    private void BuildEnemyRuntime()
    {
        enemyRuntime.Clear();

        if (G.spawnSet == null || G.spawnSet.enemies == null)
            return;

        foreach (var enemy in G.spawnSet.enemies)
        {
            if (enemy == null) continue;
            if (string.IsNullOrEmpty(enemy.id)) continue;
            if (enemyRuntime.ContainsKey(enemy.id)) continue;

            enemyRuntime.Add(enemy.id, new EnemyRuntimeData(
                enemy.id,
                enemy.startReward,
                enemy.startMaxAlive
            ));
        }
    }

    public EnemyRuntimeData GetEnemyRuntime(string enemyId)
    {
        EnsureEnemyRuntimeBuilt();

        if (string.IsNullOrEmpty(enemyId))
            return null;

        enemyRuntime.TryGetValue(enemyId, out var data);
        return data;
    }

    public int GetEnemyMaxAlive(string enemyId, int fallbackValue)
    {
        EnemyRuntimeData data = GetEnemyRuntime(enemyId);
        return data != null ? data.maxAlive : fallbackValue;
    }

    public EnemyReward GetEnemyReward(string enemyId, EnemyReward fallbackValue)
    {
        EnemyRuntimeData data = GetEnemyRuntime(enemyId);
        return data != null ? data.reward : fallbackValue;
    }

    private void EnsureEnemyRuntimeBuilt()
    {
        if (enemyRuntime.Count > 0) return;
        BuildEnemyRuntime();
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
                int finalCost = GetModifiedUpgradeCost(cost.amount);

                if (G.GetEther(cost.etherType) < finalCost)
                    return false;
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
                int finalCost = GetModifiedUpgradeCost(cost.amount);

                if (!G.SpendEther(cost.etherType, finalCost))
                    return false;
            }
        }

        ApplyEffects(levelData.effects);
        nodeLevels[nodeId] = currentLevel + 1;
        return true;
    }

    private void ApplyEffects(UpgradeEffect[] effects)
    {
        if (effects == null) return;

        foreach (var effect in effects)
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

                case UpgradeEffectType.AddEnemyMaxAlive:
                    {
                        EnemyRuntimeData data = GetEnemyRuntime(effect.stringValue);
                        if (data != null)
                            data.maxAlive += effect.intValue;
                        break;
                    }

                case UpgradeEffectType.AddEnemyReward:
                    {
                        EnemyRuntimeData data = GetEnemyRuntime(effect.stringValue);
                        if (data != null && data.reward != null)
                            data.reward.amount += effect.intValue;
                        break;
                    }

                case UpgradeEffectType.MultiplyEnemyReward:
                    {
                        EnemyRuntimeData data = GetEnemyRuntime(effect.stringValue);
                        if (data != null && data.reward != null)
                            data.reward.amount = Mathf.Max(1, Mathf.RoundToInt(data.reward.amount * effect.floatValue));
                        break;
                    }

                case UpgradeEffectType.MultiplyGlobalSpawnInterval:
                    {
                        if (G.spawner != null)
                            G.spawner.SpawnInterval *= effect.floatValue;
                        break;
                    }

            }
        }
    }

    private UpgradeNode GetNode(string nodeId)
    {
        if (treeData == null || treeData.nodes == null) return null;
        return treeData.nodes.FirstOrDefault(n => n.id == nodeId);
    }

    private bool IsUnlocked(UpgradeNode node)
    {
        return MeetsRequirements(node);
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
        return MeetsRequirements(node);
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
            int finalCost = GetModifiedUpgradeCost(cost.amount);

            switch (cost.etherType)
            {
                case EtherType.White: whiteCost += finalCost; break;
                case EtherType.Red: redCost += finalCost; break;
                case EtherType.Purple: purpleCost += finalCost; break;
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

    private int GetModifiedUpgradeCost(int baseCost)
    {
        if (G.metaUpgrades == null)
            return baseCost;

        return G.metaUpgrades.ModifyUpgradeCost(baseCost);
    }

    public void ResetForNewRitual()
    {
        nodeLevels.Clear();

        G.ClickDamage = 3;
        G.CritChance = 0.05f;
        G.CritMultiplier = 2f;
        G.ClickRadius = 0.3f;

        if (G.spawner != null)
            G.spawner.SpawnInterval = 1.6f;

        BuildEnemyRuntime();
    }

    private bool MeetsRequirements(UpgradeNode node)
    {
        if (node == null) return false;
        if (node.unlockedByDefault) return true;
        if (node.requirements == null || node.requirements.Length == 0) return true;

        bool hasValidRequirement = false;

        foreach (var requirement in node.requirements)
        {
            if (requirement == null)
                continue;

            if (string.IsNullOrEmpty(requirement.nodeId))
                continue;

            UpgradeNode requiredNode = GetNode(requirement.nodeId);
            if (requiredNode == null)
                continue;

            hasValidRequirement = true;

            int requiredLevel = Mathf.Max(1, requirement.requiredLevel);
            int currentLevel = GetCurrentLevel(requirement.nodeId);

            if (currentLevel >= requiredLevel)
                return true;
        }

        return !hasValidRequirement ? true : false;
    }
}

[System.Serializable]
public class EnemyRuntimeData
{
    public string enemyId;
    public EnemyReward reward;
    public int maxAlive;

    public EnemyRuntimeData(string enemyId, EnemyReward sourceReward, int maxAlive)
    {
        this.enemyId = enemyId;
        this.maxAlive = maxAlive;

        if (sourceReward == null)
        {
            reward = null;
            return;
        }

        reward = new EnemyReward
        {
            etherType = sourceReward.etherType,
            amount = sourceReward.amount
        };
    }
}