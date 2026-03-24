using System.Collections;
using TMPro;
using UnityEngine;

public class StoryPanelView : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private float fadeDuration = 0.2f;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetInstantVisible()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void SetInstantHidden()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        ClearBody();
    }

    public IEnumerator ShowRoutine()
    {
        if (canvasGroup == null)
            yield break;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        yield return FadeTo(1f);
    }

    public IEnumerator HideRoutine()
    {
        if (canvasGroup == null)
            yield break;

        yield return FadeTo(0f);

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        ClearBody();
    }

    public void SetBody(string text)
    {
        if (bodyText != null)
            bodyText.text = text;
    }

    public void ClearBody()
    {
        if (bodyText != null)
            bodyText.text = string.Empty;
    }

    private IEnumerator FadeTo(float target)
    {
        float start = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = fadeDuration > 0f ? time / fadeDuration : 1f;
            canvasGroup.alpha = Mathf.Lerp(start, target, t);
            yield return null;
        }

        canvasGroup.alpha = target;
    }
}