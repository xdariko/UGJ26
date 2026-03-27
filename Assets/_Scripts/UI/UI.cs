using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject finalPanel;
    [SerializeField] private GameObject upgradesPanel;

    [SerializeField] private RitualPanelUI ritualPanelUI;

    [SerializeField] private Button upgradesTreeButton;
    [SerializeField] private Button closeUpgradesTreeButton;


    [Header("Pause Panel Buttons")]
    [SerializeField] private Button exitButton;

    [Header("Final Panel Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button finalExitButton;

    private void Awake()
    {
        G.ui = this;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (finalPanel != null)
            finalPanel.SetActive(false);
    }

    private void Start()
    {
        if (upgradesTreeButton != null)
            upgradesTreeButton.onClick.AddListener(OnUpgradesTreeClicked);

        if (closeUpgradesTreeButton != null)
            closeUpgradesTreeButton.onClick.AddListener(OnCloseUpgradesTreeClicked);


        if (ritualPanelUI != null)
            ritualPanelUI.BindNextStageButton(OnNextStageClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (finalExitButton != null)
            finalExitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnCloseUpgradesTreeClicked()
    {
        if (upgradesPanel != null)
        {
            upgradesPanel.SetActive(false);
            G.IsMenuOpen = false;
        }
    }

    private void OnUpgradesTreeClicked()
    {
        if(upgradesPanel != null)
        {
            upgradesPanel.SetActive(true);
            G.IsMenuOpen = true;
        }
    }

    private void OnDestroy()
    {
        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitClicked);

        if (restartButton != null)
            restartButton.onClick.RemoveListener(OnRestartClicked);

        if (finalExitButton != null)
            finalExitButton.onClick.RemoveListener(OnExitClicked);
    }

    private void OnExitClicked()
    {
        Application.Quit();
    }

    private void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    internal void SetPausePanel(bool active)
    {
        if (pausePanel != null)
            pausePanel.SetActive(active);
    }

    public void SetFinalPanel(bool active)
    {
        if (finalPanel != null)
            finalPanel.SetActive(active);

        if (active)
            Time.timeScale = 0f;
    }

    public void SetRitualStage(string title, int whiteRequired, int redRequired, int purpleRequired, bool canAdvance)
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