using UnityEngine;
using UnityEngine.InputSystem;

public class RitualProgressionManager : MonoBehaviour
{
    [SerializeField] private RitualStageData[] stages;

    public int CurrentStageIndex { get; private set; } = 0;
    public bool IsCompleted { get; private set; }

    private void Start()
    {
        G.ritualProgression = this;

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

        var stage = stages[index];

        if (stage.objectsToActivate != null)
        {
            foreach (var obj in stage.objectsToActivate)
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

        var stage = GetCurrentStage();
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
            if (G.GetEther(req.etherType) < req.requiredAmount)
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
            bool spent = G.SpendEther(req.etherType, req.requiredAmount);
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
        var stage = GetCurrentStage();
        if (stage == null || stage.requirements == null) return 0;

        foreach (var req in stage.requirements)
        {
            if (req.etherType == etherType)
                return req.requiredAmount;
        }

        return 0;
    }

    private void UpdateUI()
    {
        if (G.ui == null) return;

        var stage = GetCurrentStage();
        if (stage == null)
        {
            G.ui.SetRitualCompleted();
            return;
        }

        bool canAdvance = CanAdvanceStage(stage);

        G.ui.SetRitualStage(stage.title,GetRequiredAmountFor(EtherType.White),GetRequiredAmountFor(EtherType.Red),GetRequiredAmountFor(EtherType.Purple),canAdvance);
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

    }
}