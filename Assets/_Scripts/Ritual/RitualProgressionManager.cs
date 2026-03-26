using UnityEngine;
using UnityEngine.InputSystem;

public class RitualProgressionManager : MonoBehaviour
{
    [SerializeField] private RitualStageData[] stages;
    [SerializeField] private RitualCompletionFlow[] completionFlows;
    public int RitualCycleIndex { get; private set; } = 0;

    public int CurrentStageIndex { get; private set; } = 0;
    public bool IsCompleted { get; private set; }

    private void Start()
    {
        G.ritualProgression = this;

        if (G.metaUpgrades != null)
            G.metaUpgrades.ResetRunOnlyCounters();

        if (stages == null || stages.Length == 0)
        {
            Debug.LogWarning("RitualProgressionManager: stages is empty.");
            return;
        }

        G.OnEtherChanged += OnEtherChanged;
        EnterStage(CurrentStageIndex);
    }

    private void OnDestroy()
    {
        G.OnEtherChanged -= OnEtherChanged;
    }

    private void Update()
    {
        if (IsCompleted) return;
        if (G.IsPaused) return;

        if (Keyboard.current != null && Keyboard.current.yKey.wasPressedThisFrame)
        {
            TryAdvanceStage();
        }
    }

    private void OnEtherChanged(EtherType type, int value)
    {
        if (IsCompleted) return;
        UpdateUI();
    }

    public RitualStageData GetCurrentStage()
    {
        if (stages == null || stages.Length == 0) return null;
        if (CurrentStageIndex < 0 || CurrentStageIndex >= stages.Length) return null;
        return stages[CurrentStageIndex];
    }

    private void EnterStage(int index)
    {
        if (index < 0 || index >= stages.Length) return;

        CurrentStageIndex = index;

        RitualStageData stage = stages[index];

        if (stage.objectsToActivate != null)
        {
            foreach (GameObject obj in stage.objectsToActivate)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }

        UpdateUI();
    }

    public void TryAdvanceStage()
    {
        if (IsCompleted) return;

        RitualStageData stage = GetCurrentStage();
        if (stage == null) return;

        if (!CanAdvanceStage(stage))
            return;

        if (!SpendRequirements(stage))
            return;

        CurrentStageIndex++;

        if (CurrentStageIndex >= stages.Length)
        {
            CompleteRitual();
            return;
        }

        EnterStage(CurrentStageIndex);
    }

    private bool CanAdvanceStage(RitualStageData stage)
    {
        if (stage.requirements == null || stage.requirements.Length == 0)
            return true;

        foreach (var req in stage.requirements)
        {
            int required = GetModifiedRequirement(req.requiredAmount);

            if (G.GetEther(req.etherType) < required)
                return false;
        }

        return true;
    }

    private bool SpendRequirements(RitualStageData stage)
    {
        if (!CanAdvanceStage(stage))
            return false;

        if (stage.requirements == null || stage.requirements.Length == 0)
            return true;

        foreach (var req in stage.requirements)
        {
            int required = GetModifiedRequirement(req.requiredAmount);

            bool spent = G.SpendEther(req.etherType, required);
            if (!spent)
            {
                Debug.LogWarning("Failed to spend ritual requirements.");
                return false;
            }
        }

        return true;
    }

    public int GetRequiredAmountFor(EtherType etherType)
    {
        RitualStageData stage = GetCurrentStage();
        if (stage == null || stage.requirements == null) return 0;

        foreach (var req in stage.requirements)
        {
            if (req.etherType == etherType)
                return GetModifiedRequirement(req.requiredAmount);
        }

        return 0;
    }

    private void UpdateUI()
    {
        if (G.ui == null) return;

        RitualStageData stage = GetCurrentStage();
        if (stage == null)
        {
            G.ui.SetRitualCompleted();
            return;
        }

        bool canAdvance = CanAdvanceStage(stage);

        G.ui.SetRitualStage(
            stage.title,
            GetRequiredAmountFor(EtherType.White),
            GetRequiredAmountFor(EtherType.Red),
            GetRequiredAmountFor(EtherType.Purple),
            canAdvance
        );
    }

    private void CompleteRitual()
    {
        IsCompleted = true;

        Debug.Log("Ritual completed!");

        if (G.ui != null)
            G.ui.SetRitualCompleted();

        OnRitualCompleted();
    }

    private void OnRitualCompleted()
    {
        RitualCompletionFlow flow = GetCompletionFlowForCurrentRitual();

        if (flow == null)
        {
            ShowMetaUpgradePanelAndRestart(true);
            return;
        }

        bool hasStory = !string.IsNullOrEmpty(flow.storyId);

        if (flow.showMetaUpgradePanel && G.metaUpgradePanel != null)
        {
            G.metaUpgradePanel.PrepareRandomChoices(() =>
            {
                StartNewRitualCycle();
            });
        }

        if (hasStory && G.storyPanel != null)
        {
            G.storyPanel.PlaySequence(flow.storyId, () =>
            {
                if (flow.showMetaUpgradePanel && G.metaUpgradePanel != null)
                {
                    G.metaUpgradePanel.ShowPrepared();
                }
                else
                {
                    StartNewRitualCycle();
                }
            });
            return;
        }

        ShowMetaUpgradePanelAndRestart(flow.showMetaUpgradePanel);
    }

    private void ShowMetaUpgradePanelAndRestart(bool shouldShow)
    {
        if (shouldShow && G.metaUpgradePanel != null)
        {
            G.metaUpgradePanel.ShowRandomChoices(() =>
            {
                StartNewRitualCycle();
            });
            return;
        }

        StartNewRitualCycle();
    }

    private RitualCompletionFlow GetCompletionFlowForCurrentRitual()
    {
        if (completionFlows == null || completionFlows.Length == 0)
            return null;

        int ritualIndex = GetCurrentRitualIndex();

        if (ritualIndex < 0 || ritualIndex >= completionFlows.Length)
            return null;

        return completionFlows[ritualIndex];
    }

    private int GetCurrentRitualIndex()
    {
        return RitualCycleIndex;
    }

    private int GetModifiedRequirement(int baseAmount)
    {
        if (G.metaUpgrades == null)
            return baseAmount;

        return G.metaUpgrades.ModifyRitualRequirement(baseAmount);
    }

    public void StartNewRitualCycle()
    {
        RitualCycleIndex++;

        IsCompleted = false;
        CurrentStageIndex = 0;

        ResetActivatedStageObjects();

        G.ResetEther();
        G.ResetUnlockedEnemies();

        if (G.metaUpgrades != null)
            G.metaUpgrades.ResetRunOnlyCounters();

        if (G.upgradeTreeManager != null)
            G.upgradeTreeManager.ResetForNewRitual();

        EnterStage(CurrentStageIndex);
    }

    private void ResetActivatedStageObjects()
    {
        if (stages == null) return;

        foreach (var stage in stages)
        {
            if (stage == null || stage.objectsToActivate == null)
                continue;

            foreach (var obj in stage.objectsToActivate)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }
}