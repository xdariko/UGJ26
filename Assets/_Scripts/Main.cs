using UnityEngine;
using UnityEngine.InputSystem;

public class Main : MonoBehaviour
{
    [SerializeField] private Transform circleCenter;
    [SerializeField] private GameObject etherOrbPrefab;
    [SerializeField] private GameObject damagePopupPrefab;
    [SerializeField] private EnemySpawnSet spawnSet;

    [Header("Imp Help")]
    [SerializeField] private float impCheckIntervalMin = 4f;
    [SerializeField] private float impCheckIntervalMax = 8f;

    private float impTimer;

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

        ResetImpTimer();
    }

    private void Update()
    {
        if (G.storyPanel == null || !G.storyPanel.IsPlaying)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                TogglePause();
            }
        }

        if (!G.IsPaused)
            UpdateImpHelp();
    }

    private void UpdateImpHelp()
    {
        if (G.metaUpgrades == null)
            return;

        impTimer -= Time.deltaTime;
        if (impTimer > 0f)
            return;

        ResetImpTimer();

        if (!G.metaUpgrades.TryGetImpEther(out int amount))
            return;

        EtherType randomType = GetRandomEtherType();
        RewardUtility.SpawnEtherOrbs(randomType, amount, G.circleCenter.position);
    }

    private EtherType GetRandomEtherType()
    {
        int value = Random.Range(0, 3);
        return (EtherType)value;
    }

    private void ResetImpTimer()
    {
        impTimer = Random.Range(impCheckIntervalMin, impCheckIntervalMax);
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