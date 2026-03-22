using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RitualPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text stageTitleText;

    [SerializeField] private GameObject whiteRoot;
    [SerializeField] private TMP_Text whiteText;

    [SerializeField] private GameObject redRoot;
    [SerializeField] private TMP_Text redText;

    [SerializeField] private GameObject purpleRoot;
    [SerializeField] private TMP_Text purpleText;

    [SerializeField] private Button nextStageButton;

    public void SetStage(string title, int whiteRequired, int redRequired, int purpleRequired, bool canAdvance)
    {
        if (nextStageButton != null)
            nextStageButton.gameObject.SetActive(false);

        if (stageTitleText != null)
            stageTitleText.text = title;

        if (whiteText != null)
        {
            if(whiteRequired != 0)
            {
                whiteRoot.SetActive(true);
                whiteText.text = $"{whiteRequired}";
            }
            else
            {
                whiteRoot.SetActive(false);
            }
        }
            

        if (redText != null)
        {
            if (redRequired != 0)
            {
                redRoot.SetActive(true);
                redText.text = $"{redRequired}";
            }
            else
            {
                redRoot.SetActive(false);
            }
        }
        

        if (purpleText != null)
        {
            if (purpleRequired != 0)
            {
                purpleRoot.SetActive(true);
                purpleText.text = $"{purpleRequired}";
            }
            else
            {
                purpleRoot.SetActive(false);
            }
        }

        if (nextStageButton != null)
            nextStageButton.gameObject.SetActive(canAdvance);
    }

    public void SetCompleted()
    {
        if (stageTitleText != null)
            stageTitleText.text = "Ритуал завершён";

        if (whiteText != null)
            whiteText.text = string.Empty;

        if (redText != null)
            redText.text = string.Empty;

        if (purpleText != null)
            purpleText.text = string.Empty;
    }

    public void BindNextStageButton(UnityEngine.Events.UnityAction action)
    {
        if (nextStageButton == null) return;

        nextStageButton.onClick.RemoveAllListeners();
        nextStageButton.onClick.AddListener(action);
    }
}

