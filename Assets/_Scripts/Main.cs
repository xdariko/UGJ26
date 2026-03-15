using UnityEngine;
using UnityEngine.InputSystem;

public class Main : MonoBehaviour
{
    [SerializeField] Transform circleCenter;
    [SerializeField] GameObject etherOrbPrefab;
    [SerializeField] private WaveSet waveSet;
    [SerializeField] private EnemySpawner spawner;
    public int StageIndex { get; private set; } = 0;

    private void Awake()
    {
        G.main = this;
        G.circleCenter = circleCenter;
        G.etherOrbPrefab = etherOrbPrefab;
    }

    private void Start()
    {
        ApplyStage(0);
    }

    
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        SetPause(!G.IsPaused);
    }

    private void SetPause(bool paused)
    {
        G.IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;

        if (G.ui != null)
            G.ui.SetPausePanel(paused);
    }

    private void ApplyStage(int stage)
    {
        if(waveSet == null || waveSet.waves == null || waveSet.waves.Length == 0)
        {
            Debug.Log("WaveSet not set");
            return;
        }

        stage = Mathf.Clamp(stage, 0, waveSet.waves.Length - 1);
        spawner.SetWave(waveSet.waves[stage]); 
    }

    private void AdvanceStage()
    {
        StageIndex++;
        if(StageIndex >= 3)
        {
            /////
            StageIndex = 0;
        }
        ApplyStage(StageIndex);
    }
}
