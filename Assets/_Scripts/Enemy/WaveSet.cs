using UnityEngine;

[CreateAssetMenu(menuName = "Game/Wave Set")]
public class WaveSet : ScriptableObject
{
    public WaveDefinition[] waves;
}