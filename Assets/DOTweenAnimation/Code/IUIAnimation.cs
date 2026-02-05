using DG.Tweening;
using System;

public interface IUIAnimation
{
    void Play();
    void Pause();
    void Resume();
    void Reverse();
    void Stop();

    void SetDuration(float duration);
    void SetEasing(Ease easeType);
    void SetLoop(int loopCount, LoopType loopType);

    bool IsPlaying { get; }
    bool IsReversed { get; }
    bool IsPaused { get; }

    event Action OnStart;
    event Action OnComplete;
    event Action OnUpdate;
}
