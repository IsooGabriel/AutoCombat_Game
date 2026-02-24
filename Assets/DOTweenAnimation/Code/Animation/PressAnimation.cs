using UnityEngine;
using UnityEngine.UI;
using static UISystem_Gabu;
public class PressAnimation : MonoBehaviour
{
    [SerializeField]
    RectTransform target;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Button button;
    [SerializeField]
    private AnimatorStatu targetState = AnimatorStatu.Pressed;
    [SerializeField]
    private float duration = 1.0f;
    [SerializeField]
    private float startPosition = 0f;
    [SerializeField]
    private float endPosition = 0f;

    private float timer = 0f;

    private void Start()
    {
        target.anchorMax = new Vector2(startPosition, target.anchorMax.y);
    }

    private void FixedUpdate()
    {
        var statu = (AnimatorStatu)UISystem_Gabu.CheckAnimationState(animator);
        if (statu == targetState)
        {
            timer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(timer / duration);
            target.anchorMax = new Vector2(Mathf.Lerp(startPosition, endPosition, t), target.anchorMax.y);
        }
        else
        {
            timer = 0f;
        }
        if(timer >= duration)
        {
            button.onClick?.Invoke();
        }
    }
}
