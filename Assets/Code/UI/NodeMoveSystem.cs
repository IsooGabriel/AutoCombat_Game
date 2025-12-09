using UnityEngine;

public class NodeMoveSystem : MonoBehaviour
{

    [SerializeField] private RectTransform followTarget;
    public RectTransform rectTransform;
    public Canvas canvas;
    public Vector2 pivotOffset = new Vector2(0f, -40f);
    private bool isDragging = true;

    public bool IsDragging { get => isDragging; set => isDragging = value; }

    public void OnClick()
    {
        IsDragging = !IsDragging;
    }

    private void Awake()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
    }

    private void Start()
    {
        Awake();
    }
    private void Update()
    {
        if (!IsDragging)
        {
            return;
        }
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out localPos
        );

        Vector2 adjustedPos = localPos + pivotOffset;

        rectTransform.anchoredPosition = adjustedPos;
    }
}
