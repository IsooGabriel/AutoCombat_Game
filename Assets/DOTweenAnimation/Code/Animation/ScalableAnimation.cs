using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScalableAnimation : UISystem_Gabu
{
    #region 変数
    [SerializeField, Header("画像")]
    protected Image image;

    [SerializeField, Header("基本のスケール")]
    protected Vector3 _normalScale = Vector3.one;
    [SerializeField, Header("カーソルが上にいる時のスケール")]
    protected Vector3 _highlightedScale = Vector3.one;
    [SerializeField, Header("押されている時のスケールの倍率")]
    protected Vector3 _pressedScale = Vector3.one;
    [SerializeField, Header("選択している時のスケールの倍率")]
    protected Vector3 _selectedScale = Vector3.one;
    [SerializeField, Header("無効な時のスケールの倍率")]
    protected Vector3 _disabledScale = Vector3.one;

    #endregion

    #region 関数
    protected override void NormalAnimation()
    {
        if (_disabledImage != null)
        {
            _disabledImage.color = new Color(0f, 0f, 0f, 0f);
        }
        _transform.localScale = Vector3.one;
        _transform.DOScale(new Vector3(_unitScale.x * _normalScale.x, _unitScale.y * _normalScale.y, _unitScale.z * _normalScale.z), _normalScaleDuration).SetEase(_normalEase);
        image.DOColor(_normalColor, _normalScaleDuration);
    }

    protected override void HighlightedAnimation()
    {
        if (_disabledImage != null)
        {
            _disabledImage.color = new Color(0f, 0f, 0f, 0f);
        }
        _transform.localScale = Vector3.one;
        _transform.DOScale(new Vector3(_unitScale.x * _highlightedScale.x, _unitScale.y * _highlightedScale.y, _unitScale.z * _highlightedScale.z), _highlightedScaleDuration).SetEase(_highlightedEase);
        image.DOColor(_highlightedColor, _highlightedScaleDuration);
    }

    protected override void PressedAnimation()
    {
        if (_disabledImage != null)
        {
            _disabledImage.color = new Color(0f, 0f, 0f, 0f);
        }
        _transform.localScale = Vector3.one;
        _transform.DOScale(new Vector3(_unitScale.x * _pressedScale.x, _unitScale.y * _pressedScale.y, _unitScale.z * _pressedScale.z), _pressedScaleDuration).SetEase(_pressedEase);
        image.DOColor(_pressedColor, _pressedScaleDuration);
    }

    protected override void SelectedAnimation()
    {
        if (_disabledImage != null)
        {
            _disabledImage.color = new Color(0f, 0f, 0f, 0f);
        }
        _transform.localScale = Vector3.one;
        _transform.DOScale(new Vector3(_unitScale.x * _selectedScale.x, _unitScale.y * _selectedScale.y, _unitScale.z * _selectedScale.z), _selectedScaleDuration).SetEase(_selectedEase);
        image.DOColor(_selectedColor, _selectedScaleDuration);
    }

    protected override void DisabledAnimation()
    {
        _transform.localScale = Vector3.one;
        _transform.DOScale(new Vector3(_unitScale.x * _disabledScale.x, _unitScale.y * _disabledScale.y, _unitScale.z * _disabledScale.z), _disabledScaleDuration).SetEase(_disabledEase);
        image.DOColor(_disabledColor, _disabledScaleDuration);

        if (_disabledImage != null)
        {
            _disabledImage.DOColor(new Color(0f, 0f, 0f, 0.7f), _disabledScaleDuration).SetEase(_disabledEase);
        }
    }

    #endregion
}
