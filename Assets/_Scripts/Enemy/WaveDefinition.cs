using UnityEngine;

public enum SpawnShape
{
    Ring,
    Rect
}

[System.Serializable]
public class WaveSpawn
{
    public GameObject prefab;

    public float weight = 1f;

    public float minRadius = 7f;
    public float maxRadius = 10f;
    public Vector2 rectHalfSize = new Vector2(10f, 6f);

    public SpawnShape spawnShape = SpawnShape.Ring;
    public int maxAlive = 999;
}

[System.Serializable]
public class WaveDefinition
{
    public WaveSpawn[] spawns;
}