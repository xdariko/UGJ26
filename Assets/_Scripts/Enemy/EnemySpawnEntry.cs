using UnityEngine;

public enum SpawnShape
{
    Ring,
    Rect
}

[System.Serializable]
public class EnemySpawnEntry
{
    [Header("Enemy")]
    public string id;
    public GameObject prefab;

    public bool unlockedByDefault = true;
    public string unlockKey;

    public EnemyReward startReward;
    public int startMaxAlive = 5;

    public float weight = 1f;
    public SpawnShape spawnShape = SpawnShape.Ring;

    public float minRadius = 7f;
    public float maxRadius = 10f;
    public Vector2 rectHalfSize = new Vector2(10f, 6f);


    public bool canTeleport = false;
    public float teleportEveryMin = 2f;
    public float teleportEveryMax = 4f;
}

