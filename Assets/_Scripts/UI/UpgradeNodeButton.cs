using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class UpgradeNodeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    [SerializeField] private UpgradeTooltipUI tooltipUI;

    private Graphic[] childGraphics;

    private void Awake()
    {
        manager = G.upgradeTreeManager;
        childGraphics = GetComponentsInChildren<Graphic>(true);

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

        if (!visible)
        {
            if (button != null)
                button.interactable = false;

            if (levelText != null)
                levelText.text = "";

            SetNodeAlpha(0f);
            return;
        }

        SetNodeAlpha(1f);

        int currentLevel = manager.GetCurrentLevel(nodeId);
        int maxLevel = manager.GetMaxLevel(nodeId);

        if (levelText != null)
            levelText.text = $"{currentLevel}/{maxLevel}";

        bool isMaxLevel = manager.IsMaxLevel(nodeId);
        bool canBuy = manager.CanBuy(nodeId);

        if (button != null)
            button.interactable = canBuy;

        if (background != null)
        {
            Color targetColor;

            if (isMaxLevel)
                targetColor = maxLevelColor;
            else if (canBuy)
                targetColor = canBuyColor;
            else
                targetColor = cannotBuyColor;

            targetColor.a = 1f;
            background.color = targetColor;
        }
    }

    private void SetNodeAlpha(float alpha)
    {
        if (childGraphics == null) return;

        foreach (var graphic in childGraphics)
        {
            if (graphic == null)
                continue;

            Color c = graphic.color;
            c.a = alpha;
            graphic.color = c;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    private void ShowTooltip()
    {
        if (tooltipUI == null || manager == null) return;
        if (!manager.ShouldNodeBeVisible(nodeId)) return;

        manager.GetCurrentLevelCosts(nodeId, out int whiteCost, out int redCost, out int purpleCost);

        tooltipUI.Show(
            manager.GetNodeTitle(nodeId),
            manager.GetNodeDescription(nodeId),
            whiteCost,
            redCost,
            purpleCost,
            transform as RectTransform
        );
    }

    private void HideTooltip()
    {
        if (tooltipUI != null)
            tooltipUI.Hide();
    }
}