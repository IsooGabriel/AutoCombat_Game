using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UIAnimationSettings
{
    public float normal = 0f;
    public float highlighted = 0f;
    public float pressed = 0f;
    public float selected = 0f;
    public float disabled = 0f;
}
public class ImageAnimation_Gabu : UISystem_Gabu
{
    #region 変数

    RectTransform rect;

    [SerializeField, Header("画像")]
    protected Image image;

    [SerializeField, Header("←→↑↓")]
    protected UIAnimationSettings[] sumSettings = {};
    protected bool _isSettingsSum = true;

    #endregion

    #region 関数
    protected override void NormalAnimation()
    {
        if (_disabledImage != null)
        {
            _disabledImage.color = new Color(0f, 0f, 0f, 0f);
        }

        _transform.DOScale(_unitScale * _normalScaleMultiplier, _normalScaleDuration).SetEase(_normalEase);
        if (_isSettingsSum && rect)
        {
            DOLeft(rect, sumSettings[0].normal, _normalScaleDuration).SetEase(_normalEase);
            DORight(rect, sumSettings[1].normal, _normalScaleDuration).SetEase(_normalEase);
            DOTop(rect, sumSettings[2].normal, _normalScaleDuration).SetEase(_normalEase);
            DOBottom(rect, sumSettings[3].normal, _normalScaleDuration).SetEase(_normalEase);
        }
        image.DOColor(_normalColor, _normalScaleDuration);
    }

    protected override void HighlightedAnimation()
    {
        if (_disabledImage != null)
        {
            _disabledImage.color = new Color(0f, 0f, 0f, 0f);
        }

        _transform.DOScale(_unitScale * _highlightedScaleMultiplier, _highlightedScaleDuration).SetEase(_highlightedEase);
        if (_isSettingsSum && rect)
        {
            for (int i = 0; i < sumSettings.Length; i++)
            {
                DOLeft(rect, sumSettings[0].highlighted, _highlightedScaleDuration).SetEase(_highlightedEase);
                DORight(rect, sumSettings[1].highlighted, _highlightedScaleDuration).SetEase(_highlightedEase);
                DOTop(rect, sumSettings[2].highlighted, _highlightedScaleDuration).SetEase(_highlightedEase);
                DOBottom(rect, sumSettings[3].highlighted, _highlightedScaleDuration).SetEase(_highlightedEase);
            }
        }
        image.DOColor(_highlightedColor, _highlightedScaleDuration);
    }

    protected override void PressedAnimation()
    {
        if (_disabledImage != null)
        {
            _disabledImage.color = new Color(0f, 0f, 0f, 0f);
        }
        Debug.Log("PressedAnimation");
        _transform.DOScale(_unitScale * _pressedScaleMultiplier, _pressedScaleDuration).SetEase(_pressedEase);
        if (_isSettingsSum && rect)
        {
            for (int i = 0; i < sumSettings.Length; i++)
            {
                DOLeft(rect, sumSettings[0].pressed, _pressedScaleDuration).SetEase(_pressedEase);
                DORight(rect, sumSettings[1].pressed, _pressedScaleDuration).SetEase(_pressedEase);
                DOTop(rect, sumSettings[2].pressed, _pressedScaleDuration).SetEase(_pressedEase);
                DOBottom(rect, sumSettings[3].pressed, _pressedScaleDuration).SetEase(_pressedEase);
            }
        }
        image.DOColor(_pressedColor, _pressedScaleDuration);
    }

    protected override void SelectedAnimation()
    {
        if (_disabledImage != null)
        {
            _disabledImage.color = new Color(0f, 0f, 0f, 0f);
        }

        _transform.DOScale(_unitScale * _selectedScaleMultiplier, _selectedScaleDuration).SetEase(_selectedEase);
        if (_isSettingsSum && rect)
        {
            for (int i = 0; i < sumSettings.Length; i++)
            {
                DOLeft(rect, sumSettings[0].selected, _selectedScaleDuration).SetEase(_selectedEase);
                DORight(rect, sumSettings[1].selected, _selectedScaleDuration).SetEase(_selectedEase);
                DOTop(rect, sumSettings[2].selected, _selectedScaleDuration).SetEase(_selectedEase);
                DOBottom(rect, sumSettings[3].selected, _selectedScaleDuration).SetEase(_selectedEase);
            }
        }
        image.DOColor(_selectedColor, _selectedScaleDuration);
    }

    protected override void DisabledAnimation()
    {
        _transform.DOScale(_unitScale * _disabledScaleMultiplier, _disabledScaleDuration).SetEase(_disabledEase);
        if (_isSettingsSum && rect)
        {
            for (int i = 0; i < sumSettings.Length; i++)
            {
                DOLeft(rect, sumSettings[0].disabled, _disabledScaleDuration).SetEase(_disabledEase);
                DORight(rect, sumSettings[1].disabled, _disabledScaleDuration).SetEase(_disabledEase);
                DOTop(rect, sumSettings[2].disabled, _disabledScaleDuration).SetEase(_disabledEase);
                DOBottom(rect, sumSettings[3].disabled, _disabledScaleDuration).SetEase(_disabledEase);
            }
        }
        image.DOColor(_disabledColor, _disabledScaleDuration);


        if (_disabledImage != null)
        {
            _disabledImage.DOColor(new Color(0f, 0f, 0f, 0.7f), _disabledScaleDuration).SetEase(_disabledEase);
        }
    }

    public void UpdateImageAnimation(ImageAnimation_Gabu imageAnimation)
    {
        base.UpdateSettings(imageAnimation);
        if (imageAnimation.image != null)
        {
            image = imageAnimation.image;
        }
        image.color = imageAnimation._normalColor;
    }

    #endregion

    // ヌルチェック、数値代入、色代入
    protected override void Start()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
            if (image == null)
            {
                Debug.LogWarning("Imageがアタッチされていません");
                return;
            }
        }
        image.color = SetNormalColor(image.color);

        rect = _transform as RectTransform;

        if (sumSettings != null && sumSettings.Length >= 4)
        {
            _isSettingsSum = true;
        }
        else if (sumSettings.Length == 1)
        {
            sumSettings = new UIAnimationSettings[4] { sumSettings[0], sumSettings[0], sumSettings[0], sumSettings[0] };
            _isSettingsSum = true;
        }
        else
        {
            _isSettingsSum = false;
        }

        base.Start();

        if (!_isReset)
        {
            return;
        }

        // Set default values
        _normalScaleDuration = 0.2f;
        _highlightedScaleDuration = 0.2f;
        _pressedScaleDuration = 0.2f;
        _selectedScaleDuration = 0.2f;
        _disabledScaleDuration = 0.3f;

        _normalScaleMultiplier = 1.0f;
        _highlightedScaleMultiplier = 1.1f;
        _pressedScaleMultiplier = 0.9f;
        _selectedScaleMultiplier = 0.97f;
        _disabledScaleMultiplier = 0.95f;

        // Set default colors
        _highlightedColor = SubtractionHSV(_normalColor, 0f, -0.4f, -0.4f);
        _pressedColor = SubtractionHSV(_normalColor, 0f, -0.2f, 0.5f);
        _selectedColor = SubtractionHSV(_normalColor, 0f, -0.2f, -0.2f);
        _disabledColor = SubtractionHSV(_normalColor, 0f, 0.7f, 0.7f);

        // Set default eases
        _normalEase = Ease.InOutSine;
        _highlightedEase = Ease.OutBack;
        _pressedEase = Ease.OutBack;
        _selectedEase = Ease.OutBack;
        _disabledEase = Ease.InOutExpo;

        _i_currentAnimation = CheckAnimationState();
        switch (_i_currentAnimation)
        {
            case (int)AnimatorStatu.Normal:
                NormalAnimation();
                break;
            case (int)AnimatorStatu.Highlighted:
                HighlightedAnimation();
                break;
            case (int)AnimatorStatu.Pressed:
                PressedAnimation();
                break;
            case (int)AnimatorStatu.Selected:
                SelectedAnimation();
                break;
            case (int)AnimatorStatu.Disabled:
                DisabledAnimation();
                break;
            default:
                Debug.LogWarning("予期しないアニメーションが参照されました");
                break;
        }
        _i_lastAnimation = _i_currentAnimation;
    }

    protected Tweener DOLeft(RectTransform rt, float endValue, float duration)
    {
        return DOTween.To(
            () => rt.offsetMin.x,
            x =>
            {
                var v = rt.offsetMin;
                v.x = x;
                rt.offsetMin = v;
            },
            endValue,
            duration
        );
    }
    protected Tweener DORight(RectTransform rt, float endValue, float duration)
    {
        return DOTween.To(
            () => rt.offsetMax.x,
            x =>
            {
                var v = rt.offsetMax;
                v.x = x;
                rt.offsetMax = v;
            },
            endValue,
            duration
        );
    }
    protected Tweener DOTop(RectTransform rt, float endValue, float duration)
    {
        return DOTween.To(
            () => rt.offsetMax.y,
            y =>
            {
                var v = rt.offsetMax;
                v.y = y;
                rt.offsetMax = v;
            },
            endValue,
            duration
        );
    }
    protected Tweener DOBottom(RectTransform rt, float endValue, float duration)
    {
        return DOTween.To(
            () => rt.offsetMin.y,
            y =>
            {
                var v = rt.offsetMin;
                v.y = y;
                rt.offsetMin = v;
            },
            endValue,
            duration
        );
    }
}
