using DG.Tweening;
using UnityEngine;

public class LoadingAnimaitonUI : MonoBehaviour
{
    [SerializeField] float jumpPower = 15;
    [SerializeField] float duration = 0.7f;
    [SerializeField] float squashAmount = 1.4f;

    Vector3 originalScale;
    Vector3 originalPos;

    void Start()
    {
        originalScale = transform.localScale;
        originalPos = transform.localPosition;

        PlayLoop();

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(duration * (0.2f / 1.2f));
        seq.Append(transform.DOLocalMoveX(originalPos.x + jumpPower * 0.7f, duration - (duration * (0.2f / 1.2f))).SetEase(Ease.Linear));
        seq.AppendInterval(duration * 0.4f);
        seq.Append(transform.DOLocalMoveX(originalPos.x, duration * (0.3f)).SetEase(Ease.OutQuad));
        seq.SetLoops(-1);
    }

    void PlayLoop()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(
            new Vector3(originalScale.x * 1.2f * squashAmount, originalScale.y * 1 / (1.2f * squashAmount), 1),
            duration * (0.2f / 1.2f)
        ));

        // 上昇
        seq.Append(transform.DOLocalMoveY(originalPos.y + jumpPower, duration * (0.5f / 1.2f))
            .SetEase(Ease.OutQuad));

        // Stretch（上昇中）
        seq.Join(transform.DOScale(
            new Vector3(originalScale.x * 1f / (1.2f * squashAmount), originalScale.y * 1.2f * squashAmount, 1),
            duration * (0.25f / 1.2f)
        ));


        // 落下
        seq.Append(transform.DOLocalMoveY(originalPos.y, duration * (0.5f / 1.2f))
            .SetEase(Ease.InQuad));

        // Squash（着地）
        seq.Join(transform.DOScale(
            new Vector3(originalScale.x, originalScale.y, 1),
            duration * 0.2f
        ));

        seq.Append(transform.DOScale(
            new Vector3(originalScale.x * 1.1f * squashAmount, originalScale.y * 1 / (1.1f * squashAmount), 1),
            duration * 0.1f
        ).SetEase(Ease.OutQuad));

        seq.Append(transform.DOScale(
            originalScale,
            duration * 0.1f
        ).SetEase(Ease.InQuad));
        seq.AppendInterval(duration * 0.5f);
        seq.SetLoops(-1);
    }
}
