using System.Collections;
using UnityEngine;

public class RandomIdleAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string triggerName = "PlayRareIdle";

    [Header("Random Delay")]
    [SerializeField] private float minDelay = 5f;
    [SerializeField] private float maxDelay = 12f;

    [Header("Conditions")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool requireObjectVisible = false;

    private Coroutine routine;
    private Renderer cachedRenderer;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        cachedRenderer = GetComponentInChildren<Renderer>();
    }

    private void OnEnable()
    {
        if (playOnStart)
            StartRandomLoop();
    }

    private void OnDisable()
    {
        StopRandomLoop();
    }

    public void StartRandomLoop()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(RandomLoop());
    }

    public void StopRandomLoop()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    private IEnumerator RandomLoop()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            if (animator == null)
                continue;

            if (requireObjectVisible && cachedRenderer != null && !cachedRenderer.isVisible)
                continue;

            animator.SetTrigger(triggerName);
        }
    }
}