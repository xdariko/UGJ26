using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private WaveDefinition currentWave;
    [SerializeField] private float spawnInterval = 0.8f;
    [SerializeField] private int maxPick = 8;
    private int[] alive;
    float t;

    public void Awake()
    {
        if (currentWave != null) SetWave(currentWave);
    }

    void Update()
    {
       if(currentWave == null || currentWave.spawns == null || currentWave.spawns.Length == 0) return;

        t += Time.deltaTime;
        if (t < spawnInterval) return;
        t = 0f;


        if (!AnySpawnAvailable()) return;

        for (int a = 0; a < maxPick; a++)
        {
            int idx = PickWeightedRandomIndex();
            if (idx < 0) return;

            var s = currentWave.spawns[idx];
            if (s.prefab == null) continue;
            if (alive[idx] >= s.maxAlive) continue;

            Spawn(idx, s);
            return;
        }
    }

    private int PickWeightedRandomIndex()
    {
        float total = 0f;
        for (int i = 0; i < currentWave.spawns.Length; i++)
        {
            float w = Mathf.Max(0f, currentWave.spawns[i].weight);
            total += w;
        }
        if (total <= 0f) return -1;

        float r = UnityEngine.Random.value * total;
        for (int i = 0; i < currentWave.spawns.Length; i++)
        {
            float w = Mathf.Max(0f, currentWave.spawns[i].weight);
            r -= w;
            if (r <= 0f) return i;
        }
        return currentWave.spawns.Length - 1;
    }

    private bool AnySpawnAvailable()
    {
        for(int i = 0; i < currentWave.spawns.Length; i++)
        {
            var s = currentWave.spawns[i];
            if(s.prefab == null) continue;
            if (alive[i] < s.maxAlive) return true;
        }
        return false;
    }

    void Spawn(int idx, WaveSpawn s)
    {
        Vector2 pos = (s.spawnShape == SpawnShape.Rect)
            ? RandomPointInRectWithMinRadius((Vector2)G.circleCenter.position, s.rectHalfSize, s.minRadius)
            : RandomPointInRing((Vector2)G.circleCenter.position, s.minRadius, s.maxRadius);

        var go = Instantiate(s.prefab, pos, Quaternion.identity);

        alive[idx]++;

        var enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.OnDied += () =>
            {
                alive[idx] = Mathf.Max(0, alive[idx] - 1);
            };
        }
    }

    static Vector2 RandomPointInRing(Vector2 center, float minRadius, float maxRadius)
    {
        float r = Mathf.Sqrt(UnityEngine.Random.Range(minRadius * minRadius, maxRadius * maxRadius));
        float a = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        return center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * r;
    }

    static Vector2 RandomPointInRectWithMinRadius(Vector2 center, Vector2 halfSize, float minRadius, int maxTries = 30)
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

    public void SetWave(WaveDefinition wave)
    {
        currentWave = wave;
        alive = (currentWave?.spawns != null) ? new int[currentWave.spawns.Length] : null;
        t = 0f;
    }
}
