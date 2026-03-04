using UnityEngine;

public class NodeCounter : MonoBehaviour
{
    [SerializeField]
    private GraphEditorManager manager;
    [SerializeField]
    private TMPro.TextMeshProUGUI text;
    [SerializeField]
    private string format1 = $"<size=80%>現在ノード数: </size>";
    [SerializeField]
    private string format2 = $"\n<size=70%>追加ステータス上限:</size><size=80%>";
    [SerializeField]
    private string format3 = $"</size>";
    private void OnChangeNodeCount()
    {
        int count = manager.nodeUIs.Count;
        text.text = $"{format1}{count}{format2}{manager.CheckMaxAdditionableStatus(count)}{format3}";
    }

    private void Start()
    {
        if (manager == null)
        {
            manager = GraphEditorManager.Instance;
            return;
        }
        manager.onChangeNodeCount += OnChangeNodeCount;
        OnChangeNodeCount();
    }
}
