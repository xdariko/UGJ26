using DG.Tweening;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHp;
    private int hp;

    private EnemyReward rewardData;
    private HitEffect hitEffect;
    private bool lastHitWasCrit;

    public event Action OnDied;

    private void Awake()
    {
        hp = maxHp;
        hitEffect = GetComponent<HitEffect>();
    }

    public void SetupReward(EnemyReward reward)
    {
        rewardData = reward;
    }

    public void TakeDamage(int dmg, bool isCrit = false)
    {
        hp -= dmg;
        lastHitWasCrit = isCrit;

        ShowDamagePopup(dmg, isCrit);

        if (hitEffect != null)
            hitEffect.ApplyHitEffect();

        if (hp <= 0)
            Die();
    }

    private void Die()
    {
        OnDied?.Invoke();

        if (rewardData != null)
            RewardUtility.GiveEnemyReward(rewardData, lastHitWasCrit, transform.position);

        Destroy(gameObject);
    }

    private void ShowDamagePopup(int dmg, bool isCrit)
    {
        if (G.damagePopupPrefab == null) return;

        Vector3 pos = transform.position + new Vector3(
            UnityEngine.Random.Range(-0.2f, 0.2f),
            0.5f,
            0f
        );

        GameObject go = Instantiate(G.damagePopupPrefab, pos, Quaternion.identity);
        DamagePopup popup = go.GetComponent<DamagePopup>();
        if (popup != null)
            popup.Setup(dmg, isCrit);
    }
}