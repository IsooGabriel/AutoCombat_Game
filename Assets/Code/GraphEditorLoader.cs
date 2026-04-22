using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using static FileSelector;
using static GraphEditorManager;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class GraphEditorLoader : MonoBehaviour
{
    public const string chanceledMessage = "ファイルの選択がキャンセルされました。⊂(^w^;";

    [SerializeField]
    private GraphEditorManager manager => GraphEditorManager.Instance;
    private readonly string graphPath = "GraphData";
    private string path;
    private GraphData graphData;

    [SerializeField]
    private GameObject loadingObject;
    [SerializeField]
    private Slider loadingSlider;

    private bool isLoading = false;


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
            ofn.lpstrFile = new string(new char[512]);
            ofn.nMaxFile = ofn.lpstrFile.Length;
            ofn.lpstrFileTitle = new string(new char[128]);
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
            ofn.lpstrTitle = "ファイルを選択";
            ofn.lpstrInitialDir = $"{Application.persistentDataPath.Replace("/", "\\")}\\{graphPath}\\";
            ofn.hwndOwner = Process.GetCurrentProcess().MainWindowHandle;
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
            else
            {
                return Task.FromResult<string>(chanceledMessage);
            }
        }
        finally
        {
            Environment.CurrentDirectory = prevDir;
        }
    }

    public async void SelectFile()
    {
        if (isLoading) return;
        isLoading = true;

        try
        {
            this.path = await OpenFileDialog();
            if (this.path == chanceledMessage)
            {
                return;
            }
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
        finally
        {
            isLoading = false;
        }
    }

    public async void LoadFunction()
    {
        if (isLoading) return;
        isLoading = true;

        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            this.path = await OpenFileDialog();
            if (this.path == GraphEditorLoader.chanceledMessage)
            {
                return;
            }
            if (string.IsNullOrEmpty(path))
            {
                string parent = Application.persistentDataPath.Replace("/", "\\");
                path =
                    $"{parent}\\{defaultPath}\\{playerDataFileName}";
            }

            loadingObject.SetActive(true);

            GraphData functionData = LoadJson(path);
            if (functionData == null)
            {
                return;
            }

            for (int i = manager.nodeUIs.Count - 1; i >= 0; --i)
            {
                loadingSlider.value += ((float)1f / Math.Max(1, manager.nodeUIs.Count)) * 0.2f * loadingSlider.maxValue;
                if (stopwatch.ElapsedMilliseconds >= 3)
                {
                    stopwatch.Restart();
                    await Task.Yield();
                }
                manager.DeleteNode(manager.nodeUIs[i]);
            }

            // マネージャーのgraphData.nodesを一度クリアするか、
            // 削除したノードが確実に消えていることを保証する必要がある。
            // DeleteNodeの中でRemoveAllするようにしたので、ここでは追加のみでOK。

            Dictionary<string, string> newIDs = new();
        NodeData node;
        for(int i = 0; i < functionData.nodes.Count; ++i)
        {
            node = functionData.nodes[i];
            loadingSlider.value += ((float)1f / manager.nodeUIs.Count) * 0.2f * loadingSlider.maxValue;

            if (node.type == NodeType.Start)
            {
                continue;
            }
            node.position += functionPosition;
            newIDs.Add(node.id, Guid.NewGuid().ToString());
            node.id = newIDs[node.id];
            manager.graphData.nodes.Add(node);
        }
        for(int i = 0; i < functionData.nodes.Count; ++i)
        {
            node = functionData.nodes[i];
            loadingSlider.value += ((float)1f / manager.nodeUIs.Count) * 0.2f * loadingSlider.maxValue;
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
        LinkedNodeData linked;
        for(int i = 0; i < functionData.linkedNodes.Count; ++i)
        {
            loadingSlider.value += ((float)1f / manager.nodeUIs.Count) * 0.4f * loadingSlider.maxValue;

            linked = functionData.linkedNodes[i];
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
        finally
        {
            isLoading = false;
        }
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

        loadingObject.SetActive(true);

        try
        {
            for (int i = 0; i < manager.graphData.nodes.Count; ++i)
            {
                loadingSlider.value += ((float)1f / manager.graphData.nodes.Count) * 0.3f * loadingSlider.maxValue;
                if (stopwatch.ElapsedMilliseconds > 3)
                {
                    stopwatch.Restart();
                    await Task.Yield();
                }

                var nodeData = manager.graphData.nodes[i];

                // 同一IDのノードUIが既に存在する場合はスキップする（二重生成防止）
                if (manager.nodeUIs.Any(ui => ui.node.id == nodeData.id))
                {
                    continue;
                }

                var nodePrefab = manager.nodePrefabs.FirstOrDefault(p => p.type == nodeData.type);
                if (nodePrefab == null || nodePrefab.prefab == null)
                {
                    Debug.LogError($"Prefab not found for node type: {nodeData.type}");
                    continue;
                }
                prefab = Instantiate(nodePrefab.prefab, new Vector3(nodeData.position.x, nodeData.position.y, 0), Quaternion.identity);
                nodeUI = prefab.GetComponent<NodeUI>();
                if (nodeUI == null)
                {
                    Debug.LogError($"NodeUI component missing on prefab for type: {nodeData.type}");
                    Destroy(prefab);
                    continue;
                }

                prefab.transform.parent = manager.nodesParent.transform;
                prefab.transform.localScale = Vector3.one;

                nodeUI.node = NodeFactory.Create(nodeData.type);
                if (nodeUI.node is GenericNode gn)
                {
                    gn.customTypeName = nodeData.customTypeName;
                }
                nodeUI.node.SetData(nodeData);
                manager.nodeUIs.Add(nodeUI);
                nodeUI.node.Initialize();
                nodeUI.node.id = nodeData.id;

                if (nodeUI is DynamicNodeUI dynamicUI)
                {
                    dynamicUI.RefreshDynamicPorts();
                }

                if (nodeUI.node is LinkedNode linked)
                {
                    var linkedData = manager.graphData.linkedNodes.FirstOrDefault(l => l.id == linked.id);
                    if (linkedData != null)
                    {
                        inputlinkeds.Add(linked, linkedData.inputNodeIDs);
                        outputlinkeds.Add(linked, linkedData.outputNodeIDs);
                    }
                }

                manager.SetPortUIsPort(nodeUI.inputPorts, nodeUI.node.inputPorts);
                manager.SetPortUIsPort(nodeUI.outputPorts, nodeUI.node.outputPorts);

                if (nodeUI is IUserVariable userVariable)
                {
                    foreach (var inputValue in nodeData.inputValues)
                    {
                        // SerializedValueからオブジェクトに復元して渡す
                        userVariable.TrySetVariable(inputValue.value.ToObject(), inputValue.toPortName);
                    }
                }


                if (prefab.TryGetComponent<NodeMoveSystem>(out var moveSystem))
                {
                    moveSystem.IsDragging = false;
                }
            }
            NodeData graphNodeData;
            for (int i = 0; i < manager.graphData.nodes.Count; ++i)
            {
                loadingSlider.value += (1f / manager.graphData.nodes.Count) * 0.3f * loadingSlider.maxValue;
                graphNodeData = manager.graphData.nodes[i];
                if (graphNodeData.outputConnections == null || graphNodeData.outputConnections.Count == 0)
                {
                    continue;
                }
                foreach (var outputConnection in graphNodeData.outputConnections)
                {
                    if (outputConnection.toPortNodes == null || outputConnection.toPortNodes.Count == 0)
                    {
                        continue;
                    }
                    var fromNodeUI = manager.nodeUIs.FirstOrDefault(n => n.node.id == graphNodeData.id);
                    if (fromNodeUI == null) continue;

                    foreach (var conection in outputConnection.toPortNodes)
                    {
                        if (stopwatch.ElapsedMilliseconds > 3)
                        {
                            stopwatch.Restart();
                            await Task.Yield();
                        }

                        var toNodeUI = manager.nodeUIs.FirstOrDefault(n => n.node.id == conection.nodeId);
                        if (toNodeUI == null) continue;

                        var fromPortUI = fromNodeUI.outputPorts.FirstOrDefault(p => p.port.portName == outputConnection.fromPortName);
                        var toPortUI = toNodeUI.inputPorts.FirstOrDefault(p => p.port.portName == conection.portName);

                        if (fromPortUI != null && toPortUI != null)
                        {
                            GraphEditorManager.ConectPorts(fromPortUI, toPortUI);
                        }
                    }
                }
            }
            NodeUI ui;
            for (int i = 0; i < manager.nodeUIs.Count; ++i)
            {
                loadingSlider.value += (1f / manager.nodeUIs.Count) * 0.39f * loadingSlider.maxValue;
                ui = manager.nodeUIs[i];
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

            loadingSlider.value = 99;

            var status = manager.graphData.aditionalStatus;
            manager.graphData.aditionalStatus = new Status()
            {
                hp = 0,
                attack = 0,
                attackCooltime = 0,
                criticalChance = 0,
                criticalDamage = 0,
            };
            manager.SetHP(status.hp);
            manager.SetAttack(status.attack);
            manager.SetAttackCT(status.attackCooltime);
            manager.SetCriticalChance(status.criticalChance);
            manager.SetCriticalDamage(status.criticalDamage);

            manager.onLoardGraph?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            loadingObject.SetActive(false);
        }
    }
}
