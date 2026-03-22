using System;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private RitualPanelUI ritualPanelUI;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        G.ui = this;
        pausePanel.SetActive(false);
    }

    private void Start()
    {
        if (ritualPanelUI != null)
            ritualPanelUI.BindNextStageButton(OnNextStageClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnExitClicked()
    {
        Application.Quit();
    }

    internal void SetPausePanel(bool active)
    {
        pausePanel.SetActive(active);
    }

    public void SetRitualStage(string title,int whiteRequired,int redRequired,int purpleRequired,bool canAdvance)
    {
        if (ritualPanelUI == null) return;

        ritualPanelUI.SetStage(title, whiteRequired, redRequired, purpleRequired, canAdvance);
    }

    public void SetRitualCompleted()
    {
        if (ritualPanelUI == null) return;
        ritualPanelUI.SetCompleted();
    }

    private void OnNextStageClicked()
    {
        if (G.ritualProgression != null)
            G.ritualProgression.TryAdvanceStage();
    }
}
