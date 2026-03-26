using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaUpgradeChoicePanel : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private MetaUpgradeCardView[] cards = new MetaUpgradeCardView[3];
    [SerializeField] private MetaUpgradeDatabase data;
    [SerializeField] private float fadeDuration = 0.2f;

    private System.Action onClosed;
    private readonly List<MetaUpgradeData> currentChoices = new List<MetaUpgradeData>();
    private Coroutine fadeRoutine;

    private void Awake()
    {
        G.metaUpgradePanel = this;
        HideInstant();
    }

    public void PrepareRandomChoices(System.Action onComplete = null)
    {
        onClosed = onComplete;

        if (data.allMetaUpgrades == null || data.allMetaUpgrades.Length == 0)
        {
            Debug.LogWarning("MetaUpgradeChoicePanel: no upgrades assigned.");
            return;
        }

        BuildRandomChoices(3);

        GameObject target = root != null ? root : gameObject;
        target.SetActive(true);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

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

    public void ShowPrepared()
    {
        GameObject target = root != null ? root : gameObject;
        target.SetActive(true);

        G.IsPaused = true;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeCanvas(0f, 1f, true, false, null));
    }

    public void ShowRandomChoices(System.Action onComplete = null)
    {
        PrepareRandomChoices(onComplete);
        ShowPrepared();
    }

    public void HideInstant()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (root != null)
            root.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    public void HideAnimated(System.Action onComplete = null)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeCanvas(1f, 0f, false, true, onComplete));
    }

    private IEnumerator FadeCanvas(float from, float to, bool interactableAtEnd, bool deactivateAtEnd, System.Action onComplete)
    {
        if (canvasGroup == null)
        {
            if (deactivateAtEnd)
            {
                if (root != null)
                    root.SetActive(false);
                else
                    gameObject.SetActive(false);
            }

            onComplete?.Invoke();
            yield break;
        }

        canvasGroup.alpha = from;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        canvasGroup.alpha = to;
        canvasGroup.interactable = interactableAtEnd;
        canvasGroup.blocksRaycasts = interactableAtEnd;

        if (deactivateAtEnd)
        {
            if (root != null)
                root.SetActive(false);
            else
                gameObject.SetActive(false);
        }

        onComplete?.Invoke();
    }

    private void OnCardSelected(MetaUpgradeData selected)
    {
        if (selected == null)
            return;

        if (G.metaUpgrades != null)
            G.metaUpgrades.ApplyUpgrade(selected);

        G.IsPaused = false;

        var callback = onClosed;
        onClosed = null;

        HideAnimated(() =>
        {
            callback?.Invoke();
        });
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