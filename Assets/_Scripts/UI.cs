using System;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private void Awake()
    {
        G.ui = this;
        pausePanel.SetActive(false);
    }

    internal void SetPausePanel(bool active)
    {
        pausePanel.SetActive(active);
    }
}
