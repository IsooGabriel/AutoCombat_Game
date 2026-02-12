using UnityEngine;
using UnityEngine.UI;

public class ScrollViewScaler : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;

    [Header("スケール設定")]
    [SerializeField] private float minScale = 0.7f;
    [SerializeField] private float maxScale = 1.0f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void Start()
    {
        scrollRect.onValueChanged.AddListener(OnScrollChanged);
        OnScrollChanged(Vector2.zero);
        foreach(RectTransform child in content)
        {
            child.GetChild(0).localScale = Vector3.one * maxScale;
        }
    }

    private void OnScrollChanged(Vector2 scrollPosition)
    {
        RectTransform viewport = scrollRect.viewport;
        Vector3 viewportCenter = viewport.position;

        foreach (RectTransform child in content)
        {
            float distance = Mathf.Abs(child.position.x - viewportCenter.x);

            float normalizedDistance = distance / (Mathf.Abs(viewportCenter.x));
            normalizedDistance = Mathf.Clamp01(normalizedDistance);

            float curveValue = scaleCurve.Evaluate(1 - normalizedDistance);
            float scale = Mathf.Lerp(minScale, maxScale, curveValue);

            child.localScale = Vector3.one * scale;
        }
    }

    private void OnDestroy()
    {
        scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
    }
}