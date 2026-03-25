using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MetaUpgradeCardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button button;
    [SerializeField] private RectTransform cardRoot;

    [Header("Hover")]
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float tweenDuration = 0.18f;

    private MetaUpgradeData data;
    private Action<MetaUpgradeData> onSelected;
    private Vector3 baseScale = Vector3.one;
    private Tween scaleTween;

    private void Awake()
    {
        if (cardRoot == null)
            cardRoot = transform as RectTransform;

        if (cardRoot != null)
            baseScale = cardRoot.localScale;

        if (button != null)
            button.onClick.AddListener(HandleClick);
    }

    public void Setup(MetaUpgradeData upgradeData, Action<MetaUpgradeData> onClick)
    {
        data = upgradeData;
        onSelected = onClick;

        if (iconImage != null)
        {
            iconImage.sprite = data != null ? data.icon : null;
            iconImage.enabled = iconImage.sprite != null;
        }

        if (titleText != null)
            titleText.text = data != null ? data.title : string.Empty;

        if (descriptionText != null)
            descriptionText.text = data != null ? data.description : string.Empty;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cardRoot == null) return;

        scaleTween?.Kill();
        scaleTween = cardRoot.DOScale(baseScale * hoverScale, tweenDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cardRoot == null) return;

        scaleTween?.Kill();
        scaleTween = cardRoot.DOScale(baseScale, tweenDuration).SetEase(Ease.OutQuad);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HandleClick();
    }

    private void HandleClick()
    {
        if (data == null) return;
        onSelected?.Invoke(data);
    }

    public void ResetScaleInstant()
    {
        scaleTween?.Kill();

        if (cardRoot != null)
            cardRoot.localScale = baseScale;
    }


}