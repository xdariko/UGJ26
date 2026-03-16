using DG.Tweening;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHp;
    public int etherReward = 1;

    int hp;

    public event Action OnDied;

    void Awake() => hp = maxHp;

    public void TakeDamage(int dmg, bool isCrit = false)
    {
        hp -= dmg;
        ShowDamagePopup(dmg, isCrit);
        if (hp <= 0) Die();
    }

    void Die()
    {
        OnDied?.Invoke();
        SpawnEtherOrbs(etherReward);
        Destroy(gameObject);
    }

    void SpawnEtherOrbs(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnOneOrb(1);
        }
    }

    void SpawnOneOrb(int v)
    {
        var startPos = transform.position;
        var orbGO = Instantiate(G.etherOrbPrefab, startPos, Quaternion.identity);

        var orb = orbGO.GetComponent<EtherOrb>();
        orb.enabled = false;
        orb.value = v;
        orb.SetTarget(G.circleCenter);

        Vector2 offset = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(0.4f, 1.2f);
        Vector3 popPos = startPos + (Vector3)offset;

        orbGO.transform.localScale = Vector3.zero;
        orbGO.transform.DOScale(1f, 0.12f).SetEase(Ease.OutBack);

        orbGO.transform.DOMove(popPos, 0.18f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                orb.enabled = true;
            });
    }

    private void ShowDamagePopup(int dmg, bool isCrit)
    {
        if (G.damagePopupPrefab == null) return;

        Vector3 pos = transform.position + new Vector3(
            UnityEngine.Random.Range(-0.2f, 0.2f),
            0.5f,
            0f
        );

        var go = Instantiate(G.damagePopupPrefab, pos, Quaternion.identity);
        var popup = go.GetComponent<DamagePopup>();
        if (popup != null)
            popup.Setup(dmg, isCrit);
    }
}
