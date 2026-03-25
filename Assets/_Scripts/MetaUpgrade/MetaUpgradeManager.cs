using UnityEngine;

public class MetaUpgradeManager : MonoBehaviour
{
    [Header("Economy")]
    public float doubleCurrencyChance = 0f;
    public float enemyRewardMultiplier = 1f;
    public int extraCritEther = 0;
    public float upgradeCostMultiplier = 1f;
    public float bonusKillEtherChance = 0f;
    public int bonusKillEtherAmount = 0;

    [Header("Kill streak reward")]
    public int greedKillStep = 0;
    public int greedKillBonus = 0;

    [Header("Ritual")]
    public float ritualRequirementMultiplier = 1f;

    [Header("Imps")]
    public float impBringEtherChance = 0f;
    public int impBringEtherAmount = 0;

    private int killCounter;

    private System.Action onClosed;

    private void Awake()
    {
        G.metaUpgrades = this;
    }

    public void ResetRunOnlyCounters()
    {
        killCounter = 0;
    }

    public void ApplyUpgrade(MetaUpgradeData upgrade)
    {
        if (upgrade == null || upgrade.effects == null)
            return;

        foreach (var effect in upgrade.effects)
        {
            switch (effect.effectType)
            {
                case MetaUpgradeType.DoubleCurrencyChance:
                    doubleCurrencyChance += effect.floatValue;
                    break;

                case MetaUpgradeType.EnemyRewardMultiplier:
                    enemyRewardMultiplier *= effect.floatValue > 0f ? effect.floatValue : 1f;
                    break;

                case MetaUpgradeType.ExtraCritEther:
                    extraCritEther += effect.intValue;
                    break;

                case MetaUpgradeType.UpgradeCostMultiplier:
                    upgradeCostMultiplier *= effect.floatValue > 0f ? effect.floatValue : 1f;
                    break;

                case MetaUpgradeType.BonusKillEtherChance:
                    bonusKillEtherChance += effect.floatValue;
                    bonusKillEtherAmount += effect.intValue;
                    break;

                case MetaUpgradeType.KillStreakBonus:
                    if (effect.intValue > 0)
                    {
                        if (greedKillStep <= 0)
                            greedKillStep = effect.intValue;
                        else
                            greedKillStep = Mathf.Min(greedKillStep, effect.intValue);
                    }

                    if (effect.floatValue > 0f)
                        greedKillBonus += Mathf.RoundToInt(effect.floatValue);
                    break;

                case MetaUpgradeType.RitualRequirementMultiplier:
                    ritualRequirementMultiplier *= effect.floatValue > 0f ? effect.floatValue : 1f;
                    break;

                case MetaUpgradeType.ImpEtherHelp:
                    impBringEtherChance += effect.floatValue;
                    impBringEtherAmount += effect.intValue;
                    break;
            }
        }
    }

    public int ModifyEnemyReward(int baseAmount)
    {
        int value = Mathf.Max(1, Mathf.RoundToInt(baseAmount * enemyRewardMultiplier));

        if (Random.value < doubleCurrencyChance)
            value *= 2;

        return value;
    }

    public int GetExtraEtherFromCrit()
    {
        return Mathf.Max(0, extraCritEther);
    }

    public int GetBonusEtherOnKill()
    {
        int extra = 0;

        if (bonusKillEtherAmount > 0 && Random.value < bonusKillEtherChance)
            extra += bonusKillEtherAmount;

        killCounter++;

        if (greedKillStep > 0 && greedKillBonus > 0 && killCounter % greedKillStep == 0)
            extra += greedKillBonus;

        return extra;
    }

    public int ModifyUpgradeCost(int baseCost)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseCost * upgradeCostMultiplier));
    }

    public int ModifyRitualRequirement(int baseValue)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseValue * ritualRequirementMultiplier));
    }

    public bool TryGetImpEther(out int amount)
    {
        amount = 0;

        if (impBringEtherAmount <= 0)
            return false;

        if (Random.value >= impBringEtherChance)
            return false;

        amount = impBringEtherAmount;
        return true;
    }
}