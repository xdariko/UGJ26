using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTooltipUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text whiteCostText;
    [SerializeField] private TMP_Text redCostText;
    [SerializeField] private TMP_Text purpleCostText;
    [SerializeField] private Image whiteIcon;
    [SerializeField] private Image redIcon;
    [SerializeField] private Image purpleIcon;

    [SerializeField] private Vector2 offset = new Vector2(220f, 0f);

    public void Show(string title, string description, int whiteCost, int redCost, int purpleCost, RectTransform target)
    {
        if (root == null) return;

        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;

        if (whiteCostText != null)
        {
            if (whiteCost != 0)
            {
                whiteIcon.enabled = true;
                whiteCostText.enabled = true;
                whiteCostText.text = whiteCost.ToString();
            }
            else
            {
                whiteIcon.enabled = false;
                whiteCostText.enabled = false;
            }
        }

        if (redCostText != null)
        {
            if (redCost != 0)
            {
                redIcon.enabled = true;
                redCostText.enabled = true;
                redCostText.text = redCost.ToString();
            }
            else
            {
                redIcon.enabled = false;
                redCostText.enabled = false;
            }
        }

        if (purpleCostText != null)
        {
            if (purpleCost != 0)
            {
                purpleIcon.enabled = true;
                purpleCostText.enabled = true;
                purpleCostText.text = purpleCost.ToString();
            }
            else
            {
                purpleIcon.enabled = false;
                purpleCostText.enabled = false;
            }
        }

        PositionNear(target);
        root.SetActive(true);
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }

    private void PositionNear(RectTransform target)
    {
        if (target == null || root == null) return;

        RectTransform tooltipRect = root.GetComponent<RectTransform>();
        if (tooltipRect == null) return;

        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);

        Vector3 rightMiddle = (corners[2] + corners[3]) * 0.5f;
        tooltipRect.position = rightMiddle + (Vector3)offset;
    }
}