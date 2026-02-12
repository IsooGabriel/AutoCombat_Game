using DG.Tweening;
using System;
using UnityEngine;

public class MoveRectAnimation : MonoBehaviour, IUIAnimation
{
    #region •Ï”

    [SerializeField]
    private bool playOnStart = true;
    [SerializeField]
    private bool playOnEnable = false;
    [SerializeField]
    private bool playOnDisable = false;

    private Tween _tween;
    private RectTransform rectTransform => transform as RectTransform;

    [SerializeField] Vector2 _startPosition;
    [SerializeField] Vector2 _endPosition;

    [Header("StartAnchor")]
    [SerializeField] Vector2 _startMinAnchor;
    [SerializeField] Vector2 _startMaxAnchor;
    [Header("EndAnchor")]
    [SerializeField] Vector2 _endMinAnchor;
    [SerializeField] Vector2 _endMaxAnchor;
    [SerializeField] bool _useAnchorPosition = false;

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

    #region ŠÖ”

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

    public void FlipAndPlay()
    {
        if(isReversed)
        {
            Play();
        }
        else
        {
            Reverse();
        }
    }

    private Tween CreateAnimation()
    {
        if (_useAnchorPosition)
        {
            Sequence sequence = DOTween.Sequence();

            rectTransform.anchorMax = _startMaxAnchor;
            rectTransform.anchorMin = _startMinAnchor;
            sequence.Join(rectTransform.DOAnchorMax(_endMaxAnchor, _duration)
                .SetEase(_easeType)
                .SetLoops(_loopCount, _loopType)
                .OnComplete(() => OnComplete?.Invoke())
                .OnUpdate(() => OnUpdate?.Invoke()));
            sequence.Join(rectTransform.DOAnchorMin(_endMinAnchor, _duration)
                .SetEase(_easeType)
                .SetLoops(_loopCount, _loopType)
                .OnComplete(() => OnComplete?.Invoke())
                .OnUpdate(() => OnUpdate?.Invoke()));
            return sequence;
        }
        else
        {
            rectTransform.anchoredPosition = _startPosition;

            return rectTransform.DOAnchorPos(_endPosition, _duration)
                .SetEase(_easeType)
                .SetLoops(_loopCount, _loopType)
                .OnComplete(() => OnComplete?.Invoke())
                .OnUpdate(() => OnUpdate?.Invoke());
        }
    }

    private Tween CreateReverseAnimation()
    {
        if (_useAnchorPosition)
        {
            Sequence sequence = DOTween.Sequence();

            rectTransform.anchorMax = _endMaxAnchor;
            rectTransform.anchorMin = _endMinAnchor;
            sequence.Join(rectTransform.DOAnchorMax(_startMaxAnchor, _duration)
                .SetEase(_easeType)
                .SetLoops(_loopCount, _loopType)
                .OnComplete(() => OnComplete?.Invoke())
                .OnUpdate(() => OnUpdate?.Invoke()));
            sequence.Join(rectTransform.DOAnchorMin(_startMinAnchor, _duration)
                .SetEase(_easeType)
                .SetLoops(_loopCount, _loopType)
                .OnComplete(() => OnComplete?.Invoke())
                .OnUpdate(() => OnUpdate?.Invoke()));
            return sequence;
        }
        else
        {
            rectTransform.anchoredPosition = _endPosition;

            return rectTransform.DOAnchorPos(_startPosition, _duration)
                .SetEase(_easeType)
                .SetLoops(_loopCount, _loopType)
                .OnComplete(() => OnComplete?.Invoke())
                .OnUpdate(() => OnUpdate?.Invoke());
        }
    }

    async public void WaitForCompletion()
    {
        if (_tween != null)
        {
            await _tween.AsyncWaitForCompletion();
        }
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
        if (playOnEnable)
        {
            Play();
        }
    }

    private void OnDisable()
    {
        if (playOnDisable)
        {
            Play();
            WaitForCompletion();
        }
    }
}
