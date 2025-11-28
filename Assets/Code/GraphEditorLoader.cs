using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class GraphEditorLoader : MonoBehaviour
{
    [SerializeField]
    private GraphEditorManager manager;
    private string path;
    private GraphData graphData;

    private void Start()
    {
        string path = Application.persistentDataPath.Replace("/", "\\");
        System.Diagnostics.Process.Start(
        "explorer.exe",
        path
        );
    }

    public void LoadJson(string path)
    {
        this.path = path;
        string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
        graphData = JsonUtility.FromJson<GraphData>(json);
    }

    public void LoadEditor()
    {
        GameObject prefab = null;
        NodeUI nodeUI = null;
        foreach (var nodeData in graphData.nodes)
        {
            prefab = manager.nodePrefabs.First(p => p.type == nodeData.type).prefab;
            if (prefab == null)
            {
                continue;
            }
            prefab = Instantiate(prefab, new Vector3(nodeData.position.x, nodeData.position.y, 0), Quaternion.identity);
            nodeUI = prefab.GetComponent<NodeUI>();
            if (prefab == null)
            {
                continue;
            }
            if (nodeUI == null)
            {
                Destroy(prefab);
                continue;
            }
            prefab.transform.parent = manager.nodesParent.transform;
            nodeUI.node.SetData(nodeData);
            manager.nodeUIs.Add(nodeUI);
        }
    }
}
