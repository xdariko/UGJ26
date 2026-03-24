using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemySpawnSet spawnSet;
    [SerializeField] private float spawnInterval = 1.6f;
    [SerializeField] private int maxPick = 8;

    public float SpawnInterval
    {
        get => spawnInterval;
        set => spawnInterval = Mathf.Max(0.05f, value);
    }

    private int[] alive;
    private float t;

    private void Awake()
    {
        G.spawner = this;
        Init();
    }

    private void OnEnable()
    {
        G.OnEnemyUnlocked += OnEnemyUnlocked;
    }

    private void OnDisable()
    {
        G.OnEnemyUnlocked -= OnEnemyUnlocked;
    }

    private void Init()
    {
        alive = (spawnSet != null && spawnSet.enemies != null)
            ? new int[spawnSet.enemies.Length]
            : null;

        t = 0f;
    }

    private void OnEnemyUnlocked(string key)
    {

    }

    private void Update()
    {
        if (spawnSet == null || spawnSet.enemies == null || spawnSet.enemies.Length == 0)
            return;

        t += Time.deltaTime;
        if (t < spawnInterval) return;
        t = 0f;

        if (!AnySpawnAvailable()) return;

        for (int a = 0; a < maxPick; a++)
        {
            int idx = PickWeightedRandomIndex();
            if (idx < 0) return;

            var s = spawnSet.enemies[idx];
            if (!CanSpawn(idx, s)) continue;

            Spawn(idx, s);
            return;
        }
    }

    private bool CanSpawn(int idx, EnemySpawnEntry s)
    {
        if (s == null) return false;
        if (s.prefab == null) return false;
        if (!G.IsEnemyUnlocked(s.unlockKey, s.unlockedByDefault)) return false;
        if (alive == null || idx < 0 || idx >= alive.Length) return false;
        int maxAlive = s.startMaxAlive;

        if (G.upgradeTreeManager != null)
            maxAlive = G.upgradeTreeManager.GetEnemyMaxAlive(s.id, s.startMaxAlive);

        if (alive[idx] >= maxAlive) return false;

        return true;
    }

    private bool AnySpawnAvailable()
    {
        for (int i = 0; i < spawnSet.enemies.Length; i++)
        {
            if (CanSpawn(i, spawnSet.enemies[i]))
                return true;
        }

        return false;
    }

    private int PickWeightedRandomIndex()
    {
        float total = 0f;

        for (int i = 0; i < spawnSet.enemies.Length; i++)
        {
            var s = spawnSet.enemies[i];
            if (!CanSpawn(i, s)) continue;

            float w = Mathf.Max(0f, s.weight);
            total += w;
        }

        if (total <= 0f) return -1;

        float r = UnityEngine.Random.value * total;

        for (int i = 0; i < spawnSet.enemies.Length; i++)
        {
            var s = spawnSet.enemies[i];
            if (!CanSpawn(i, s)) continue;

            float w = Mathf.Max(0f, s.weight);
            r -= w;
            if (r <= 0f) return i;
        }

        return spawnSet.enemies.Length - 1;
    }

    private void Spawn(int idx, EnemySpawnEntry s)
    {
        Vector2 center = G.circleCenter.position;

        Vector2 pos = (s.spawnShape == SpawnShape.Rect)
            ? RandomPointInRectWithMinRadius(center, s.rectHalfSize, s.minRadius)
            : RandomPointInRing(center, s.minRadius, s.maxRadius);

        var go = Instantiate(s.prefab, pos, Quaternion.identity);

        alive[idx]++;

        var enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            EnemyReward reward = s.startReward;
            if (G.upgradeTreeManager != null)
                reward = G.upgradeTreeManager.GetEnemyReward(s.id, s.startReward);
            enemy.SetupReward(reward);

            enemy.OnDied += () =>
            {
                alive[idx] = Mathf.Max(0, alive[idx] - 1);
            };
        }

        var teleport = go.GetComponent<EnemyTeleport>();
        if (teleport != null) 
        {
            teleport.Setup(s);
        }
    }

    public void SetSpawnSet(EnemySpawnSet newSet)
    {
        spawnSet = newSet;
        Init();
    }

    public static Vector2 RandomPointInRing(Vector2 center, float minRadius, float maxRadius)
    {
        float r = Mathf.Sqrt(UnityEngine.Random.Range(minRadius * minRadius, maxRadius * maxRadius));
        float a = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        return center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * r;
    }

    public static Vector2 RandomPointInRectWithMinRadius(Vector2 center, Vector2 halfSize, float minRadius, int maxTries = 30)
    {
        float minR2 = minRadius * minRadius;

        for (int i = 0; i < maxTries; i++)
        {
            float x = Random.Range(center.x - halfSize.x, center.x + halfSize.x);
            float y = Random.Range(center.y - halfSize.y, center.y + halfSize.y);
            Vector2 p = new Vector2(x, y);

            if ((p - center).sqrMagnitude >= minR2)
                return p;
        }

        float a = Random.Range(0f, Mathf.PI * 2f);
        return center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * minRadius;
    }
}