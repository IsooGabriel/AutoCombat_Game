using DG.Tweening;
using System;
using UnityEngine;

public class ButtonOutlineAnimation_Gabu : ImageAnimation_Gabu
{
    #region 変数
    #endregion

    #region 関数
    protected override void NormalAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }

        image.DOColor(_normalColor, _normalScaleDuration).SetEase(_normalEase);
    }

    protected override void HighlightedAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }

        image.DOColor(_highlightedColor, _highlightedScaleDuration).SetEase(_highlightedEase);
    }

    protected override void PressedAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }

        image.DOColor(_pressedColor, _pressedScaleDuration).SetEase(_pressedEase);
    }

    protected override void SelectedAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }

        image.DOColor(_selectedColor, _selectedScaleDuration).SetEase(_selectedEase);
    }

    protected override void DisabledAnimation()
    {
        if (_i_currentAnimation == _i_lastAnimation)
        {
            return;
        }

        image.DOColor(_disabledColor, _disabledScaleDuration).SetEase(_disabledEase);
    }
    #endregion 
}
