using System.Collections;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private float dissolveTime = 0.5f;

    private int dissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private int verticalDissolveAmount = Shader.PropertyToID("_VerticalDissolve");

    private SpriteRenderer[] spriteRenderers;
    private Material[] materials;

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        materials = new Material[spriteRenderers.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = spriteRenderers[i].material;
        }
    }

    public Coroutine PlayVanish(MonoBehaviour runner, bool useDissolve = true, bool useVertical = false)
    {
        return runner.StartCoroutine(Vanish(useDissolve, useVertical));
    }

    public Coroutine PlayAppear(MonoBehaviour runner, bool useDissolve = true, bool useVertical = false)
    {
        return runner.StartCoroutine(Appear(useDissolve, useVertical));
    }

    private IEnumerator Vanish(bool useDissolve, bool useVertical)
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(0f, 1.5f, (elapsedTime / dissolveTime));
            float lerpedVerticalDissolve = Mathf.Lerp(0f, 1.1f, (elapsedTime / dissolveTime));

            for (int i = 0; i < materials.Length; i++)
            {
                if (useDissolve)
                    materials[i].SetFloat(dissolveAmount, lerpedDissolve);

                if (useVertical)
                    materials[i].SetFloat(verticalDissolveAmount, lerpedVerticalDissolve);
            }
            yield return null;
        }
    }

    private IEnumerator Appear(bool useDissolve, bool useVertical)
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(1.5f, 0f, (elapsedTime / dissolveTime));
            float lerpedVerticalDissolve = Mathf.Lerp(1.1f, 0f, (elapsedTime / dissolveTime));

            for (int i = 0; i < materials.Length; i++)
            {
                if (useDissolve)
                    materials[i].SetFloat(dissolveAmount, lerpedDissolve);

                if (useVertical)
                    materials[i].SetFloat(verticalDissolveAmount, lerpedVerticalDissolve);
            }
            yield return null;
        }
    }
}
