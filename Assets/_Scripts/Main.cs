using UnityEngine;
using UnityEngine.InputSystem;

public class Main : MonoBehaviour
{
    [SerializeField] Transform circleCenter;
    [SerializeField] GameObject etherOrbPrefab;
    [SerializeField] GameObject damagePopupPrefab;
    [SerializeField] private EnemySpawnSet spawnSet;

    public int StageIndex { get; private set; } = 0;

    private void Awake()
    {
        G.main = this;
        G.circleCenter = circleCenter;
        G.etherOrbPrefab = etherOrbPrefab;
        G.damagePopupPrefab = damagePopupPrefab;
        G.spawnSet = spawnSet;
    }

    private void Start()
    {
        Debug.Log($"storyPanel = {G.storyPanel}");
        G.storyPanel?.PlaySequence("intro_01");
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
}
