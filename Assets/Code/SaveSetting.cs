using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSetting : MonoBehaviour
{
    [SerializeField]
    private GraphEditorManager manager;
    [SerializeField]
    private Toggle isCustomPath;
    [SerializeField]
    private TextMeshProUGUI path;
    [SerializeField]
    private TextMeshProUGUI graphName;
    [SerializeField]
    private TextMeshProUGUI author;

    public void OnClick()
    {
        if (manager == null)
        {
            return;
        }

        if (isCustomPath.isOn)
        {
            manager.SaveGraph(path.text, graphName.text, author.text);
        }
        else
        {
            manager.SaveGraph(graphName:graphName.text, author:author.text);
        }
    }
}
