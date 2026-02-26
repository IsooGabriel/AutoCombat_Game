using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private RectTransform rectTransform;
    [SerializeField]
    private GameObject[] enableOnClick = { };
    [SerializeField]
    private GameObject[] disableOnClick = { };

    public async void OnClick(InputAction.CallbackContext context)
    {
        bool isContains = RectTransformUtility.RectangleContainsScreenPoint(
            rectTransform,
            Input.mousePosition,
            canvas.worldCamera);

        if (!isContains)
        {
            return;
        }
        gameObject.SetActive(false);
        await Task.Delay(100);
        foreach (var obj in enableOnClick)
        {
            obj.SetActive(true);
        }
        foreach (var obj in disableOnClick)
        {
            obj.SetActive(false);
        }
    }

    private void OnEnable()
    {
        playerInput.actions["Click"].performed += OnClick;
    }
    private void OnDisable()
    {
        playerInput.actions["Click"].performed -= OnClick;
    }
}
