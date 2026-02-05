using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UISystem_Gabu : ColorSystem
{
    #region 変数
    protected bool _isButton = false;
    protected bool _isEexecute = true;
    protected int _i_currentAnimation = 0;
    protected int _i_lastAnimation = -1;

    public bool IsChangeable = false;
    public int setStatu = 0;

    [SerializeField]
    protected Animator _animator;

    [SerializeField]
    protected Transform _transform;
    [SerializeField]
    protected Vector3 _unitPosition = Vector3.one;
    [SerializeField]
    protected Vector3 _unitRotation = Vector3.one;
    [SerializeField]
    protected Vector3 _unitScale = Vector3.one;

    [SerializeField, Header("基本色")]
    protected Color _normalColor = new Color(0.8823529f, 0.8823529f, 0.8823529f, 1f);
    [SerializeField, Header("カーソルが上にいる時の色")]
    protected Color _highlightedColor = Color.white;
    [SerializeField, Header("押されている時の色")]
    protected Color _pressedColor = Color.white;
    [SerializeField, Header("選択している時の色")]
    protected Color _selectedColor = Color.white;
    [SerializeField, Header("無効な時の色")]
    protected Color _disabledColor = new Color(0.7019608f, 0.7019608f, 0.7019608f, 1f);

    [SerializeField, Header("UI色を取得して基本色を変える")]
    protected bool _isGetColor = false;
    [SerializeField, Header("基本色を元に他の色を変える")]
    protected bool _isAutoColor = false;
    [SerializeField, Header("H(色相)が変更されなくなる")]
    protected bool _isLockHue = true;
    [SerializeField, Header("S(彩度)gaが変更されなくなる、モノクロになります")]
    protected bool _isLockSaturation = true;
    [SerializeField, Header("V(明度)が変更されなくなる")]
    protected bool _isLockValue = true;


    [SerializeField, Header("Disabledの時に使われる画像")]
    protected Image _disabledImage;

    [SerializeField, Header("実行時リセット")]
    protected bool _isReset = false;
    [SerializeField, Header("--- 各種アニメーション変数 ---")]
    protected float _normalScaleDuration = 0.4f;
    [SerializeField]
    protected float _highlightedScaleDuration = 0.2f;
    [SerializeField]
    protected float _pressedScaleDuration = 0f;
    [SerializeField]
    protected float _selectedScaleDuration = 0.2f;
    [SerializeField]
    protected float _disabledScaleDuration = 0.05f;

    [SerializeField]
    protected Ease _normalEase = Ease.InOutSine;
    [SerializeField]
    protected Ease _highlightedEase = Ease.OutBack;
    [SerializeField]
    protected Ease _pressedEase = Ease.InBack;
    [SerializeField]
    protected Ease _selectedEase = Ease.Linear;
    [SerializeField]
    protected Ease _disabledEase = Ease.Linear;

    [SerializeField]
    protected float _normalScaleMultiplier = 1.0f;
    [SerializeField]
    protected float _highlightedScaleMultiplier = 1.1f;
    [SerializeField]
    protected float _pressedScaleMultiplier = 1.05f;
    [SerializeField]
    protected float _selectedScaleMultiplier = 1.1f;
    [SerializeField]
    protected float _disabledScaleMultiplier = 1.0f;


    public enum AnimatorStatu : int
    {
        Normal = 0, Highlighted, Pressed, Selected, Disabled
    }
    #endregion

    #region 関数
    /// <summary>
    /// Animatorの再生中のアニメーションを確認します。
    /// </summary>
    /// <returns></returns>
    protected int CheckAnimationState()
    {
        if (_animator == null)
        {
            return setStatu;
        }
        if (_isButton || _animator == null)
        {
            return (int)AnimatorStatu.Normal;
        }

        foreach (string state in Enum.GetNames(typeof(AnimatorStatu)))
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(state))
            {
                return (int)Enum.Parse(typeof(AnimatorStatu), state);
            }
        }

        return (int)AnimatorStatu.Normal;
    }


    protected virtual void NormalAnimation()
    {
        _transform.DOScale(_unitScale, _normalScaleDuration).SetEase(_normalEase);
    }

    protected virtual void HighlightedAnimation()
    {
        _transform.DOScale(_unitScale * _highlightedScaleMultiplier, _highlightedScaleDuration).SetEase(_highlightedEase);
    }

    protected virtual void PressedAnimation()
    {
        _transform.DOScale(_unitScale * _pressedScaleMultiplier, _pressedScaleDuration).SetEase(_pressedEase);
    }

    protected virtual void SelectedAnimation()
    {
        _transform.DOScale(_unitScale * _selectedScaleMultiplier, _selectedScaleDuration).SetEase(_selectedEase);
    }

    protected virtual void DisabledAnimation()
    {
        _transform.DOScale(_unitScale, _disabledScaleDuration).SetEase(_disabledEase);
    }

    public void UpdateAnimation(int _i_currentAnimation)
    {
        if (_i_lastAnimation == _i_currentAnimation)
        {
            return;
        }
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

    /// <summary>
    /// 外部からステータスを変更します。isChangeableがfalseの場合は変更できません。
    /// </summary>
    /// <param name="newStatu">実行するアニメーションのステータス</param>
    public void ChangeStatu(AnimatorStatu newStatu)
    {
        if (!IsChangeable)
        {
            return;
        }
        UpdateAnimation((int)newStatu);
    }

    public virtual void UpdateSettings(UISystem_Gabu uiSystem)
    {
        if (uiSystem._animator != null)
        {
            _animator = uiSystem._animator;
        }
        if (uiSystem._transform != null)
        {
            _transform = uiSystem._transform;
        }
        _unitPosition = uiSystem._unitPosition;
        _unitRotation = uiSystem._unitRotation;
        _unitScale = uiSystem._unitScale == Vector3.zero ? _unitScale : uiSystem._unitScale;
        _normalColor = uiSystem._normalColor;
        _highlightedColor = uiSystem._highlightedColor;
        _pressedColor = uiSystem._pressedColor;
        _selectedColor = uiSystem._selectedColor;
        _disabledColor = uiSystem._disabledColor;
        _isGetColor = uiSystem._isGetColor;
        _isAutoColor = uiSystem._isAutoColor;
        _isLockHue = uiSystem._isLockHue;
        _isLockSaturation = uiSystem._isLockSaturation;
        _isLockValue = uiSystem._isLockValue;
        if (uiSystem._disabledImage != null)
        {
            _disabledImage = uiSystem._disabledImage;
        }
        _isReset = uiSystem._isReset;
        _normalScaleDuration = uiSystem._normalScaleDuration;
        _highlightedScaleDuration = uiSystem._highlightedScaleDuration;
        _pressedScaleDuration = uiSystem._pressedScaleDuration;
        _selectedScaleDuration = uiSystem._selectedScaleDuration;
        _disabledScaleDuration = uiSystem._disabledScaleDuration;
        _normalEase = uiSystem._normalEase;
        _highlightedEase = uiSystem._highlightedEase;
        _pressedEase = uiSystem._pressedEase;
        _selectedEase = uiSystem._selectedEase;
        _disabledEase = uiSystem._disabledEase;
        _normalScaleMultiplier = uiSystem._normalScaleMultiplier;
        _highlightedScaleMultiplier = uiSystem._highlightedScaleMultiplier;
        _pressedScaleMultiplier = uiSystem._pressedScaleMultiplier;
        _selectedScaleMultiplier = uiSystem._selectedScaleMultiplier;
        _disabledScaleMultiplier = uiSystem._disabledScaleMultiplier;
    }

    protected Color SetNormalColor(Color UICol)
    {
        if (_isGetColor)
        {
            _normalColor *= UICol;
            return _normalColor;
        }
        else return _normalColor;
    }
    #endregion

    protected virtual void Start()
    {
        if (_isAutoColor)
        {

            Color HSV_H;
            Color.RGBToHSV(_highlightedColor, out HSV_H.r, out HSV_H.g, out HSV_H.b);
            Color HSV_P;
            Color.RGBToHSV(_pressedColor, out HSV_P.r, out HSV_P.g, out HSV_P.b);
            Color HSV_S;
            Color.RGBToHSV(_selectedColor, out HSV_S.r, out HSV_S.g, out HSV_S.b);
            Color HSV_D;
            Color.RGBToHSV(_disabledColor, out HSV_D.r, out HSV_D.g, out HSV_D.b);
            if (!_isLockHue)
            {
                HSV_H.r = GetHue(_normalColor);
                HSV_P.r = GetHue(_normalColor);
                HSV_S.r = GetHue(_normalColor);
                HSV_D.r = GetHue(_normalColor);
            }
            if (!_isLockSaturation)
            {
                HSV_H.g = GetSaturation(_normalColor);
                HSV_P.g = GetSaturation(_normalColor);
                HSV_S.g = GetSaturation(_normalColor);
                HSV_D.g = GetSaturation(_normalColor);
            }
            if (!_isLockValue)
            {
                HSV_H.b = GetValue(_normalColor);
                HSV_P.b = GetValue(_normalColor);
                HSV_S.b = GetValue(_normalColor);
                HSV_D.b = GetValue(_normalColor);
            }
            _highlightedColor = Color.HSVToRGB(HSV_H.r, HSV_H.g, HSV_H.b);
            _pressedColor = Color.HSVToRGB(HSV_P.r, HSV_P.g, HSV_P.b);
            _selectedColor = Color.HSVToRGB(HSV_S.r, HSV_S.g, HSV_S.b);
            _disabledColor = Color.HSVToRGB(HSV_D.r, HSV_D.g, HSV_D.b);
        }
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        if (_transform == null)
        {
            _transform = GetComponent<Transform>();
        }
        if (_unitPosition != _transform.position)
        {
            _unitPosition = _transform.position;
        }
        if (_unitRotation != _transform.eulerAngles)
        {
            _unitRotation = _transform.eulerAngles;
        }
        if (_unitScale != _transform.localScale)
        {
            _unitScale = _transform.localScale;
        }
        _i_lastAnimation = -1;
    }

    protected virtual void Update()
    {

        if (_animator == null)
        {
            return;
        }

        if (IsChangeable)
        {
            return;
        }

        _i_currentAnimation = CheckAnimationState();
        UpdateAnimation(_i_currentAnimation);
    }
}
