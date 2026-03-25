using DG.Tweening;
using UnityEngine;

public static class RewardUtility
{
    public static void GiveEnemyReward(EnemyReward reward, bool wasCrit, Vector3 worldPosition)
    {
        if (reward == null)
            return;

        int amount = reward.amount;

        if (G.metaUpgrades != null)
            amount = G.metaUpgrades.ModifyEnemyReward(amount);

        SpawnEtherOrbs(reward.etherType, amount, worldPosition);

        if (wasCrit && G.metaUpgrades != null)
        {
            int critBonus = G.metaUpgrades.GetExtraEtherFromCrit();
            if (critBonus > 0)
                SpawnEtherOrbs(reward.etherType, critBonus, worldPosition);
        }

        if (G.metaUpgrades != null)
        {
            int killBonus = G.metaUpgrades.GetBonusEtherOnKill();
            if (killBonus > 0)
                SpawnEtherOrbs(reward.etherType, killBonus, worldPosition);
        }
    }

    public static void SpawnEtherOrbs(EtherType type, int amount, Vector3 worldPosition)
    {
        if (amount <= 0) return;

        for (int i = 0; i < amount; i++)
        {
            SpawnOneOrb(type, 1, worldPosition);
        }
    }

    private static void SpawnOneOrb(EtherType type, int value, Vector3 startPos)
    {
        if (G.etherOrbPrefab == null || G.circleCenter == null)
            return;

        GameObject orbGO = Object.Instantiate(G.etherOrbPrefab, startPos, Quaternion.identity);

        EtherOrb orb = orbGO.GetComponent<EtherOrb>();
        if (orb == null)
        {
            Object.Destroy(orbGO);
            return;
        }

        orb.enabled = false;
        orb.Setup(type, value, G.circleCenter);

        Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(0.4f, 1.2f);
        Vector3 popPos = startPos + (Vector3)offset;
        Vector3 targetScale = orb.transform.localScale;

        orbGO.transform.localScale = Vector3.zero;
        orbGO.transform.DOScale(targetScale, 0.1f).SetEase(Ease.OutBack);

        orbGO.transform.DOMove(popPos, 0.18f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                orb.enabled = true;
            });
    }
}