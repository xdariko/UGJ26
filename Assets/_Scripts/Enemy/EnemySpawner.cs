using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;

    float t;

    void Update()
    {
        t += Time.deltaTime;
        if(t >= spawnInterval)
        {
            t = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        var pos = (Vector2)transform.position + Random.insideUnitCircle.normalized * 10f;
        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}
