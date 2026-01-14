using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddNodeButtonFuctory : MonoBehaviour
{
    [SerializeField]
    private NodeType nodeType;
    [SerializeField]
    private Button button;
    [SerializeField]
    private bool changeText = false;
    [SerializeField]
    private TextMeshProUGUI text;

    public void Awake()
    {
        if (button == null)
        {
            button = GetComponentInChildren<Button>();
        }
        button?.onClick.RemoveAllListeners();
        button?.onClick.AddListener(() => GraphEditorManager.Instance.AddNode(nodeType));

        if (text == null)
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }
        if(changeText && text != null)
        {
            text.text = $"<size=20%>+</size>{nodeType.ToString()}";
        }
    }
}