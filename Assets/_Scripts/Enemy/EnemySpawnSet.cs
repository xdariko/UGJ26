using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy Spawn Set")]
public class EnemySpawnSet : ScriptableObject
{
    public EnemySpawnEntry[] enemies;
}