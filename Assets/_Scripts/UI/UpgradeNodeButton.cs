using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class UpgradeNodeButton : MonoBehaviour
{
    [SerializeField] private string nodeId;
    private UpgradeTreeManager manager;

    [SerializeField] private Button button;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text levelText;

    [SerializeField] private GameObject[] connectorsToShowWhenWisible;

    [SerializeField] private Color canBuyColor = Color.yellow;
    [SerializeField] private Color cannotBuyColor = Color.gray;
    [SerializeField] private Color maxLevelColor = Color.green;

    private void Awake()
    {
        manager = G.upgradeTreeManager;

        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    private void Update()
    {
        if (manager == null) return;
        Refresh();
    }

    private void Refresh()
    {
        bool visible = manager.ShouldNodeBeVisible(nodeId);

        SetVisible(connectorsToShowWhenWisible, visible);
        if (!visible) return;

        int currentLevel = manager.GetCurrentLevel(nodeId);
        int maxLevel = manager.GetMaxLevel(nodeId);

        if (levelText != null) levelText.text = $"{currentLevel}/{maxLevel}";

        bool isMaxLevel = manager.IsMaxLevel(nodeId);
        bool canBuy = manager.CanBuy(nodeId);

        if (button != null) button.interactable = canBuy;

        if (background != null)
        {
            if (isMaxLevel)
                background.color = maxLevelColor;
            else if (canBuy)
                background.color = canBuyColor;
            else
                background.color = cannotBuyColor;

        }
    }

    private void OnClick()
    {
        if (manager == null) return;

        if (manager.Buy(nodeId))
            Refresh();
    }

    private void SetVisible(GameObject[] objects, bool value)
    {
        if (objects == null) return;

        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(value);
        }
    }
}

