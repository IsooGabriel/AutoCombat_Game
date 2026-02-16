using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using static GraphEditorManager;
using static FileSelector;

public class GraphEditorLoader : MonoBehaviour
{
    [SerializeField]
    private GraphEditorManager manager => GraphEditorManager.Instance;
    private readonly string graphPath = "GraphData";
    private string path;
    private GraphData graphData;

    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName(ref OpenFileName ofn);



    public Task<string> OpenFileDialog()
    {
        string prevDir = Environment.CurrentDirectory;
        try
        {
            OpenFileName ofn = new OpenFileName();
            ofn.lStructSize = Marshal.SizeOf(ofn);
            ofn.lpstrFilter = "グラフデータ\0*.json;*.acjson\0";
            ofn.lpstrFile = new string(new char[256]);
            ofn.nMaxFile = ofn.lpstrFile.Length;
            ofn.lpstrFileTitle = new string(new char[64]);
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
            ofn.lpstrTitle = "ファイルを選択";
            ofn.lpstrInitialDir = $"{Application.persistentDataPath.Replace("/", "\\")}\\{graphPath}\\";

            if (!Directory.Exists(ofn.lpstrInitialDir))
            {
                Directory.CreateDirectory(ofn.lpstrInitialDir);
            }
            if (GetOpenFileName(ref ofn))
            {
                if (!File.Exists(ofn.lpstrFile))
                {
                    File.WriteAllText(ofn.lpstrFile, "", System.Text.Encoding.UTF8);
                }
                return Task.FromResult<string>(ofn.lpstrFile);
            }
            return null;
        }
        finally
        {
            Environment.CurrentDirectory = prevDir;
        }
    }

    public async void SelectFile()
    {
        this.path = await OpenFileDialog();
        if (string.IsNullOrEmpty(path))
        {
            string parent = Application.persistentDataPath.Replace("/", "\\");
            path =
                $"{parent}\\{defaultPath}\\{playerDataFileName}";
        }
        manager.ResetGraph();
        manager.graphData = LoadJson(path);
        LoadEditor();
    }

    public async void LoadFunction()
    {
        this.path = await OpenFileDialog();
        if (string.IsNullOrEmpty(path))
        {
            string parent = Application.persistentDataPath.Replace("/", "\\");
            path =
                $"{parent}\\{defaultPath}\\{playerDataFileName}";
        }
        GraphData fanctionData = LoadJson(path);
        foreach(var node in fanctionData.nodes)
        {
            if (node.type == NodeType.Start)
            {
                continue;
            }
            manager.graphData.nodes.Add(node);
        }
        fanctionData.linkedNodes.ForEach(n => manager.graphData.linkedNodes.Add(n));
        if(manager.graphData.author != fanctionData.author)
        {
            manager.graphData.author += nameSpacer + fanctionData.author;
        }
        if(manager.graphData.graphName != fanctionData.graphName)
        {
            manager.graphData.graphName += nameSpacer + fanctionData.graphName;
        }
        LoadEditor();
    }

    public GraphData LoadJson(string path)
    {
        this.path = path;
        string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
        return JsonUtility.FromJson<GraphData>(json);
    }

    public void LoadEditor()
    {
        GameObject prefab = null;
        NodeUI nodeUI = null;
        foreach (var nodeData in manager.graphData.nodes)
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
            prefab.transform.localScale = Vector3.one;
            nodeUI.node = NodeFactory.Create(nodeData.type);
            nodeUI.node.SetData(nodeData);
            manager.nodeUIs.Add(nodeUI);
            nodeUI.node.Initialize();
            nodeUI.node.id = nodeData.id;
            manager.SetPortUIsPort(nodeUI.inputPorts, nodeUI.node.inputPorts);
            manager.SetPortUIsPort(nodeUI.outputPorts, nodeUI.node.outputPorts);

            if (nodeUI is IUserVariable userVariable)
            {
                foreach (var inputValue in nodeData.inputValues)
                {
                    userVariable.TrySetVariable((float)inputValue.value, inputValue.toPortName);
                }
            }

            
            if (prefab.TryGetComponent<NodeMoveSystem>(out var moveSystem))
            {
                moveSystem.IsDragging = false;
            }
        }
        foreach (var nodeData in manager.graphData.nodes)
        {
            if (nodeData.outputConnections == null || nodeData.outputConnections.Count == 0)
            {
                continue;
            }
            foreach (var outputConnection in nodeData.outputConnections)
            {
                if (outputConnection.toPortNodes == null || outputConnection.toPortNodes.Count == 0)
                {
                    continue;
                }
                var fromNodeUI = manager.nodeUIs.First(n => n.node.id == nodeData.id);
                foreach (var conection in outputConnection.toPortNodes)
                {
                    var toNodeUI = manager.nodeUIs.First(n => n.node.id == conection.nodeId);
                    GraphEditorManager.ConectPorts(
                        fromNodeUI.outputPorts.First(p => p.port.portName == outputConnection.fromPortName),
                        toNodeUI.inputPorts.First(p => p.port.portName == conection.portName)
                    );
                }
            }
        }
        GraphEditorManager.Instance.onLoardGraph?.Invoke();
    }

}
