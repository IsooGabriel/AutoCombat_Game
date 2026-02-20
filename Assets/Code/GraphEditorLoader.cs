using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static FileSelector;
using static GraphEditorManager;

public class GraphEditorLoader : MonoBehaviour
{
    [SerializeField]
    private GraphEditorManager manager => GraphEditorManager.Instance;
    private readonly string graphPath = "GraphData";
    private string path;
    private GraphData graphData;

    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName(ref OpenFileName ofn);

    private Vector2 functionPosition = Vector2.zero;
    public void SetFunctionPositionX(string x)
    {
        functionPosition.x = float.Parse(x);
    }
    public void SetFunctionPositionY(string y)
    {
        functionPosition.y = float.Parse(y);
    }


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
        await LoadEditor();
    }

    public async void LoadFunction()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        this.path = await OpenFileDialog();
        if (string.IsNullOrEmpty(path))
        {
            string parent = Application.persistentDataPath.Replace("/", "\\");
            path =
                $"{parent}\\{defaultPath}\\{playerDataFileName}";
        }
        GraphData functionData = LoadJson(path);
        if (functionData == null)
        {
            return;
        }

        for (int i = 0; i < manager.nodeUIs.Count; ++i)
        {
            var targetID = manager.nodeUIs[i].node.id;
            for (int j = 0; j < manager.graphData.nodes.Count; ++j)
            {
                if (false && stopwatch.ElapsedMilliseconds >= 3)
                {
                    stopwatch.Restart();
                    await Task.Yield();
                }
                manager.DeleteNode(manager.nodeUIs[i]);
            }
        }

        Dictionary<string, string> newIDs = new();
        foreach (var node in functionData.nodes)
        {
            if (node.type == NodeType.Start)
            {
                continue;
            }
            node.position += functionPosition;
            newIDs.Add(node.id, Guid.NewGuid().ToString());
            node.id = newIDs[node.id];
            manager.graphData.nodes.Add(node);
        }
        foreach (var node in functionData.nodes)
        {
            foreach (var port in node.outputConnections)
            {
                if (port.toPortNodes == null || port.toPortNodes.Count == 0)
                {
                    continue;
                }
                foreach (var toPortNode in port.toPortNodes)
                {
                    if (stopwatch.ElapsedMilliseconds > 3)
                    {
                        stopwatch.Restart();
                        await Task.Yield();
                    }
                    if (newIDs.ContainsKey(toPortNode.nodeId))
                    {
                        toPortNode.nodeId = newIDs[toPortNode.nodeId];
                    }
                }
            }
        }
        foreach (var linked in functionData.linkedNodes)
        {
            linked.inputNodeIDs.ForEach(id => id = newIDs[id]);
            linked.outputNodeIDs.ForEach(id => id = newIDs[id]);
            manager.graphData.linkedNodes.Add(linked);
        }
        if (manager.graphData.author != functionData.author)
        {
            manager.graphData.author += nameSpacer + functionData.author;
        }
        if (manager.graphData.graphName != functionData.graphName)
        {
            manager.graphData.graphName += nameSpacer + functionData.graphName;
        }

        await LoadEditor();
    }

    public GraphData LoadJson(string path)
    {
        this.path = path;
        string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
        var data = JsonUtility.FromJson<GraphData>(json);
        foreach (var node in data.nodes)
        {
            if (float.Parse(data.version) < 0.2f)
            {
                node.position = new Vector2(node.position.x * 100, node.position.y * 100);
            }
        }
        return data;
    }

    public async Task LoadEditor()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        GameObject prefab = null;
        NodeUI nodeUI = null;
        Dictionary<LinkedNode, List<string>> inputlinkeds = new Dictionary<LinkedNode, List<string>>();
        Dictionary<LinkedNode, List<string>> outputlinkeds = new Dictionary<LinkedNode, List<string>>();

        for (int i = 0; i < manager.graphData.nodes.Count; ++i)
        {
            if (stopwatch.ElapsedMilliseconds > 3)
            {
                stopwatch.Restart();
                await Task.Yield();
            }

            var nodeData = manager.graphData.nodes[i];

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
            if (nodeUI.node is LinkedNode linked)
            {
                inputlinkeds.Add(linked, manager.graphData.linkedNodes.First(l => l.id == linked.id).inputNodeIDs);
                outputlinkeds.Add(linked, manager.graphData.linkedNodes.First(l => l.id == linked.id).outputNodeIDs);
            }

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
                    if (stopwatch.ElapsedMilliseconds > 3)
                    {
                        stopwatch.Restart();
                        await Task.Yield();
                    }

                    var toNodeUI = manager.nodeUIs.First(n => n.node.id == conection.nodeId);
                    GraphEditorManager.ConectPorts(
                        fromNodeUI.outputPorts.First(p => p.port.portName == outputConnection.fromPortName),
                        toNodeUI.inputPorts.First(p => p.port.portName == conection.portName)
                    );
                }
            }
        }
        foreach (var ui in manager.nodeUIs)
        {

            if (stopwatch.ElapsedMilliseconds > 3)
            {
                stopwatch.Restart();
                await Task.Yield();
            }
            foreach (var linked in inputlinkeds)
            {
                if (linked.Value.Contains(ui.node.id))
                {
                    linked.Key.toNodes.ToList().AddRange(new Node[] { ui.node });
                }
            }
            foreach (var linked in outputlinkeds)
            {
                if (linked.Value.Contains(ui.node.id))
                {
                    linked.Key.toNodes.ToList().AddRange(new Node[] { ui.node });
                }
            }
        }
        GraphEditorManager.Instance.onLoardGraph?.Invoke();
    }

}
