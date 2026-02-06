using DG.Tweening;
using System;
using UnityEngine;

public class InstructionsSpaceAnimation : MonoBehaviour, IUIAnimation
{
    #region 変数

    [SerializeField]
    private bool playOnStart = true;

    private Tween _tween;
    private RectTransform rectTransform => transform as RectTransform;

    [SerializeField] Vector2 _startPosition;
    [SerializeField] Vector2 _endPosition;
    [SerializeField] float _duration = 0.3f;
    [SerializeField] Ease _easeType = Ease.Linear;
    [SerializeField] int _loopCount = 0;
    [SerializeField] LoopType _loopType = LoopType.Restart;

    public bool IsPlaying => _tween != null && _tween.IsActive() && _tween.IsPlaying();
    public bool IsPaused => _tween != null && _tween.IsActive() && !_tween.IsPlaying();
    public bool IsReversed => _tween != null && _tween.IsActive() && isReversed;
    private bool isReversed = false;

    public event Action OnStart;
    public event Action OnComplete;
    public event Action OnUpdate;
     
    #endregion

    #region 関数

    public void Play()
    {
        if (_tween == null || isReversed)
        {
            _tween = CreateAnimation();
        }
        isReversed = false;

        _tween.Play();
        OnStart?.Invoke();
    }

    public void Pause()
    {
        _tween?.Pause();
    }

    public void Resume()
    {
        _tween?.Play();
    }

    public void Reverse()
    {
        if (_tween == null || !isReversed)
        {
            _tween = CreateReverseAnimation();
        }
        isReversed = true;

        _tween.Play();
        OnStart?.Invoke();
    }

    public void Stop()
    {
        _tween?.Kill();
        _tween = null;
    }

    public void SetDuration(float duration)
    {
        _duration = duration;
        RestartTween();
    }

    public void SetEasing(Ease easeType)
    {
        _easeType = easeType;
        RestartTween();
    }

    public void SetLoop(int loopCount, LoopType loopType)
    {
        _loopCount = loopCount;
        _loopType = loopType;
        RestartTween();
    }

    public void RestartTween()
    {
        if (_tween != null)
        {
            _tween.Kill();
        }
        _tween = CreateAnimation();
        isReversed = false;
    }

    private Tween CreateAnimation()
    {
        rectTransform.anchoredPosition = _startPosition;

        return rectTransform.DOAnchorPos(_endPosition, _duration)
            .SetEase(_easeType)
            .SetLoops(_loopCount, _loopType)
            .OnComplete(() => OnComplete?.Invoke())
            .OnUpdate(() => OnUpdate?.Invoke());
    }

    private Tween CreateReverseAnimation()
    {
        rectTransform.anchoredPosition = _endPosition;

        return rectTransform.DOAnchorPos(_startPosition, _duration)
            .SetEase(_easeType)
            .SetLoops(_loopCount, _loopType)
            .OnComplete(() => OnComplete?.Invoke())
            .OnUpdate(() => OnUpdate?.Invoke());
    }

    #endregion

    private void Start()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }
}
