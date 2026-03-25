using System.Collections.Generic;
using UnityEngine;

public class MetaUpgradeChoicePanel : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private MetaUpgradeCardView[] cards = new MetaUpgradeCardView[3];
    [SerializeField] private MetaUpgradeDatabase data;
    private System.Action onClosed;

    private readonly List<MetaUpgradeData> currentChoices = new List<MetaUpgradeData>();

    private void Awake()
    {
        G.metaUpgradePanel = this;
        HideInstant();
    }

    public void ShowRandomChoices(System.Action onComplete = null)
    {
        onClosed = onComplete;
        if (data.allMetaUpgrades == null || data.allMetaUpgrades.Length == 0)
        {
            Debug.LogWarning("MetaUpgradeChoicePanel: no upgrades assigned.");
            return;
        }

        BuildRandomChoices(3);

        if (root != null)
            root.SetActive(true);
        else
            gameObject.SetActive(true);

        G.IsPaused = true;

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null)
                continue;

            cards[i].gameObject.SetActive(i < currentChoices.Count);
            cards[i].ResetScaleInstant();

            if (i < currentChoices.Count)
                cards[i].Setup(currentChoices[i], OnCardSelected);
        }
    }

    public void HideInstant()
    {
        if (root != null)
            root.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    private void OnCardSelected(MetaUpgradeData selected)
    {
        if (selected == null)
            return;

        if (G.metaUpgrades != null)
            G.metaUpgrades.ApplyUpgrade(selected);

        G.IsPaused = false;
        HideInstant();

        var callback = onClosed;
        onClosed = null;
        callback?.Invoke();
    }

    private void BuildRandomChoices(int count)
    {
        currentChoices.Clear();

        List<MetaUpgradeData> pool = new List<MetaUpgradeData>();
        for (int i = 0; i < data.allMetaUpgrades.Length; i++)
        {
            if (data.allMetaUpgrades[i] != null)
                pool.Add(data.allMetaUpgrades[i]);
        }

        int pickCount = Mathf.Min(count, pool.Count);

        for (int i = 0; i < pickCount; i++)
        {
            int index = Random.Range(0, pool.Count);
            currentChoices.Add(pool[index]);
            pool.RemoveAt(index);
        }
    }
}