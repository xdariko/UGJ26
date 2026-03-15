using DG.Tweening;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Enemy : MonoBehaviour
{
    public int maxHp;
    public int etherReward = 1;

    int hp;

    void Awake() => hp = maxHp;

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0) Die();
    }

    void Die()
    {
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

        Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(0.4f, 1.2f);
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
}
