using System.Collections;
using UnityEngine;

public class EnemyTeleport :MonoBehaviour
{
    [SerializeField] private bool enabledTeleport;
    [SerializeField] private float teleportEveryMin = 2f;
    [SerializeField] private float teleportEveryMax = 4f;

    [SerializeField] private bool useDissolve = true;
    [SerializeField] private bool useVertical = false;

    private SpawnShape spawnShape;
    private float minRadius;
    private float maxRadius;
    private Vector2 rectHalfSize;

    private Dissolve dissolve;
    private Enemy enemy;

    private Coroutine teleportRoutine;
    private bool isTeleporting;

    private bool isDead;

    private void Start()
    {
        if (enemy != null)
            enemy.OnDied += HandleDeath;
    }

    private void Awake()
    {
        dissolve = GetComponent<Dissolve>();
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        
    }
    private void OnDestroy()
    {
        if (enemy != null)
            enemy.OnDied -= HandleDeath;
    }

    private void HandleDeath()
    {
        isDead = true;
        enabledTeleport = false;
        StopAllCoroutines();
    }

    public void Setup(EnemySpawnEntry data)
    {
        enabledTeleport = data.canTeleport;
        teleportEveryMin = data.teleportEveryMin;
        teleportEveryMax = data.teleportEveryMax;
        spawnShape=data.spawnShape;
        minRadius=data.minRadius;
        maxRadius=data.maxRadius;
        rectHalfSize = data.rectHalfSize;

        if (teleportRoutine != null)
            StopCoroutine(teleportRoutine);

        if (enabledTeleport)
            teleportRoutine = StartCoroutine(TeleportLoop());
    }

    private IEnumerator TeleportLoop()
    {
        while (enabledTeleport)
        {
            float delay = Random.Range(teleportEveryMin, teleportEveryMax);
            yield return new WaitForSeconds(delay);

            if (isTeleporting) continue;

            yield return TeleportOnce();
        }
    }

    private IEnumerator TeleportOnce()
    {
        isTeleporting = true;

        if(dissolve != null)
        {
            yield return dissolve.PlayVanish(this, useDissolve, useVertical);
        }

        Vector2 center = G.circleCenter.position;
        Vector2 newPos = GetTeleportPoint(center);

        transform.position = newPos;

        if (dissolve != null)
        {
            yield return dissolve.PlayAppear(this, useDissolve, useVertical);
        }

        isTeleporting = false;
    }

    private Vector2 GetTeleportPoint(Vector2 center)
    {
        Vector2 current = transform.position;

        for (int i = 0; i < 20; i++)
        {
            Vector2 p = spawnShape == SpawnShape.Rect
                ? RandomPointInRectWithMinRadius(center, rectHalfSize, minRadius)
                : RandomPointInRing(center, minRadius, maxRadius);

            if ((p - current).sqrMagnitude < 1.5f * 1.5f)
                continue;

            return p;
        }

        return current;
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

