using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhaseImage : MonoBehaviour
{
    [SerializeField] private Image backGroundImage;
    [SerializeField] private Sprite enableImage;

    [SerializeField] private TMP_Text phaseText;
    [SerializeField] private Color activeTextColor;
    [SerializeField] private Color inactiveTextColor = Color.white;

    private CanvasGroup canvasGroup;
    public bool IsActivePhase { get; private set; }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void UpdatePhaseColorAlpha(float alpha)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = alpha;
    }

    public void SetBackgroundSprite(bool isActive)
    {
        backGroundImage.sprite = isActive ? enableImage : null;
    }

    public void SetRootWidth(float width)
    {
        var rt = GetComponent<RectTransform>();
        if (rt != null)
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    public Tween AnimateRootWidth(float targetWidth, float duration = 0.3f)
    {
        var rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            return DOTween.To(
                () => rt.rect.width,
                w => rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w),
                targetWidth,
                duration
            ).SetEase(Ease.OutQuad);
        }
        return null;
    }

    public void UpdateTextColor(bool isActive)
    {
        if (phaseText != null)
            phaseText.color = isActive ? activeTextColor : inactiveTextColor;
    }

    public void DOKickPhase(bool isActive, float targetWidth, Vector3 scale, float delay, bool isInitial)
    {
        IsActivePhase = isActive;

        SetBackgroundSprite(isActive);
        UpdatePhaseColorAlpha(isActive ? 1f : 0.5f);
        UpdateTextColor(isActive);

        var backRT = backGroundImage.rectTransform;
        var textTR = phaseText.rectTransform;

        if (isInitial)
        {
            phaseText.gameObject.SetActive(false);
            textTR.localScale = Vector3.one;

            AnimateRootWidth(targetWidth, 0.3f)?.SetDelay(delay).SetEase(Ease.OutQuad);

            backRT.DOScale(scale, 0.4f).SetDelay(delay).SetEase(Ease.OutBack);

            DOVirtual.DelayedCall(delay + 0.35f, () =>
            {
                phaseText.gameObject.SetActive(true);
                phaseText.transform.localScale = Vector3.zero;

                phaseText.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
            });
        }
        else
        {
            AnimateRootWidth(targetWidth, 0.25f)?.SetEase(Ease.OutQuad);
            backRT.DOScale(scale, 0.25f).SetEase(Ease.OutQuad);
        }
    }
}
