using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimation_Gabu : UISystem_Gabu
{
    #region 変数
    [SerializeField, Header("画像")]
    protected Image image;

    #endregion

    #region 関数
    protected override void NormalAnimation()
    {
        if (_disabledImage != null)
        {
            _disabledImage.color = new Color(0f, 0f, 0f, 0f);
        }

        _transform.DOScale(_unitScale * _normalScaleMultiplier, _normalScaleDuration).SetEase(_normalEase);
        image.DOColor(_normalColor, _normalScaleDuration);
    }

    protected override void HighlightedAnimation()
    {
        if (_disabledImage != null)
        {
            _disabledImage.color = new Color(0f, 0f, 0f, 0f);
        }

        _transform.DOScale(_unitScale * _highlightedScaleMultiplier, _highlightedScaleDuration).SetEase(_highlightedEase);
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
        image.DOColor(_pressedColor, _pressedScaleDuration);
    }

    protected override void SelectedAnimation()
    {
        if (_disabledImage != null)
        {
            _disabledImage.color = new Color(0f, 0f, 0f, 0f);
        }

        _transform.DOScale(_unitScale * _selectedScaleMultiplier, _selectedScaleDuration).SetEase(_selectedEase);
        image.DOColor(_selectedColor, _selectedScaleDuration);
    }

    protected override void DisabledAnimation()
    {
        _transform.DOScale(_unitScale * _disabledScaleMultiplier, _disabledScaleDuration).SetEase(_disabledEase);
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
}
