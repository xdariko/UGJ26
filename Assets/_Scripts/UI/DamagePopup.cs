using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class DamagePopup: MonoBehaviour
{
    [SerializeField] private TextMeshPro text;

    public void Setup(int damage, bool isCrit)
    {
        text.text = damage.ToString();

        if (isCrit)
        {
            text.color = Color.yellow;
            text.fontSize *= 1.25f;
        }
        else
        {
            text.color = Color.white;
        }

        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 1.2f, 0f);

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(end, 0.6f).SetEase(Ease.OutQuad));
        seq.Join(transform.DOScale(isCrit ? 1.2f : 1f, 0.15f).From(0.6f));
        seq.Join(text.DOFade(0f, 0.6f));
        seq.OnComplete(() => Destroy(gameObject));
    }
}

