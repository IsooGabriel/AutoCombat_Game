using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GraphEditorManager : MonoBehaviour
{
    public Camera graphCamera;

    [NonSerialized]
    public readonly string editorLayer = "GraphEditor";

    static public GraphEditorManager Instance = null;
    [NonSerialized]
    public bool isSelected = false;
    [NonSerialized]
    public PortUI selectedPort = null;
    private float lineWidth = 1f;
    [NonSerialized]
    public List<NodeUI> nodeUIs = new();
    public GameObject nodesParent;
    public NodePrefab[] nodePrefabs;
    public GraphData graphData = new();

    public readonly float portUISaturation = 0.7f;
    public readonly float portUIValue = 0.8f;
    public readonly float portUISerectSaturation = 0.25f;
    public readonly float portUISerectValue = 0.9f;

    public TMP_InputField additionalHP;
    public TMP_InputField additionalAttack;
    public TMP_InputField additionalAttackCT;
    public TMP_InputField additionalCriticalChance;
    public TMP_InputField additionalCriticalDamage;

    public PlayerInput playerInput;
    public EventSystem eventSystem;
    public GraphicRaycaster raycaster;

    public const string defaultPath = "GraphData";
    public const string enemyPath = "EnemyGraph";
    public const string playerDataFileName = "player.acjson";
    public const string enemyDataFileName = "enemy.json";
    public const string nameSpacer = ", ";
    public readonly int[] maxNodesForPoints = { 987, 610, 377, 233, 144, 89, 55, 34, 21, 13 };
    private int aditionableStatusCount = 0;
    private GraphEditorLoader loader;
    public Action onLoardGraph;

    #region 関数

    public void AddNode(NodeType type)
    {
        Node node = NodeFactory.Create(type);
        node.EditorInitialize();
        node.position = Vector2.zero;

        NodeUI nodeUI = Instantiate(
            GraphEditorManager.Instance.nodePrefabs.FirstOrDefault(p => p.type == type).prefab,
            GraphEditorManager.Instance.nodesParent.transform
        ).GetComponent<NodeUI>();
        nodeUI.node = node;
        nodeUI.transform.position = node.position;

        SetPortUIsPort(nodeUI.inputPorts, node.inputPorts);
        SetPortUIsPort(nodeUI.outputPorts, node.outputPorts);

        Instance.nodeUIs.Add(nodeUI);
        Instance.AjustAdditionalStatus(Instance.graphData.aditionalStatus);
        Instance.UpdateAdditionalStatus();
    }
    public void SetPortUIsPort(PortUI[] portUIs, Port[] ports)
    {
        for (int i = 0; i < portUIs.Length; ++i)
        {
            if (ports == null || ports.Length <= i || ports[i] == null)
            {
                if (portUIs[i] != null)
                {
                    Destroy(portUIs[i].gameObject);
                }
                continue;
            }
            portUIs[i].port = ports[i];
        }
    }
    public void AddNode(string typeName)
    {
        NodeType type;
        Enum.TryParse(typeName, out type);
        AddNode(type);
    }


    /// <summary>
    /// カーソル下のノードを削除
    /// </summary>
    /// <param name="context"></param>
    public void DeleteNode(InputAction.CallbackContext context)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        if (results.Count == 0)
        {
            return;
        }

        GameObject topUI = results[0].gameObject;
        PortUI portUI = topUI.GetComponentInParent<PortUI>();
        if (portUI != null)
        {
            DisconectPort(portUI);
            return;
        }

        NodeUI nodeUI = topUI.GetComponentInParent<NodeUI>();
        if (nodeUI.node is StartNode)
        {
            return;
        }

        DeleteNode(nodeUI);
    }

    public void DeleteNode(NodeUI nodeUI)
    {
        Node deleteNode = nodeUI.node;

        Instance.nodeUIs.Remove(nodeUI);

        foreach (var nodeDeta in Instance.nodeUIs)
        {
            foreach (var port in nodeDeta.node.outputPorts)
            {
                port.outputConections.RemoveAll(c => c.node.id == deleteNode.id);
            }
        }

        Destroy(nodeUI.gameObject);
    }

    public void ResetGraph()
    {
        graphData = null;
        foreach (var nodeUI in nodeUIs)
        {
            Destroy(nodeUI.gameObject);
        }
        nodeUIs.Clear();
        aditionableStatusCount = 0;
    }

    #region 追加ステータス

    public void UpdateAdditionalStatus()
    {
        additionalHP.text = $"{Instance.graphData.aditionalStatus.hp}";
        additionalAttack.text = $"{Instance.graphData.aditionalStatus.attack}";
        additionalAttackCT.text = $"{Instance.graphData.aditionalStatus.attackCooltime}";
        additionalCriticalChance.text = $"{Instance.graphData.aditionalStatus.criticalChance}";
        additionalCriticalDamage.text = $"{Instance.graphData.aditionalStatus.criticalDamage}";
    }

    /// <summary>
    /// アディショナルステータスを加算する
    /// </summary>
    /// <param name="value">加算する量</param>
    /// <param name="status">実際のアディショナルステータス</param>
    public void SetAdditionalStatus(int value, ref int status)
    {
        if (value < 0)
        {
            value = 0;
        }
        int old = status;
        if (aditionableStatusCount - old + value > Instance.CheckMaxAdditionableStatus())
        {
            value = Instance.CheckMaxAdditionableStatus() - (aditionableStatusCount - old);
        }
        aditionableStatusCount += value - old;
        status = value;
        Instance.UpdateAdditionalStatus();
    }

    public void SetHP(int value)
    {
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.hp);
    }
    public void SetHP(string value)
    {
        Instance.SetHP(int.Parse(value));
    }
    public void AddHP(int value)
    {
        value += Instance.graphData.aditionalStatus.hp;
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.hp);
    }


    public void SetAttack(int value)
    {
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.attack);
    }
    public void SetAttack(string value)
    {
        Instance.SetAttack(int.Parse(value));
    }
    public void AddAttack(int value)
    {
        value += Instance.graphData.aditionalStatus.attack;
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.attack);
    }


    public void SetAttackCT(int value)
    {
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.attackCooltime);
    }
    public void SetAttackCT(string value)
    {
        Instance.SetAttackCT(int.Parse(value));
    }

    public void AddAttackCT(int value)
    {
        value += Instance.graphData.aditionalStatus.attackCooltime;
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.attackCooltime);
    }


    public void SetCriticalChance(int value)
    {
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.criticalChance);
    }
    public void SetCriticalChance(string value)
    {
        Instance.SetCriticalChance(int.Parse(value));
    }
    public void AddCriticalChance(int value)
    {
        value += Instance.graphData.aditionalStatus.criticalChance;
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.criticalChance);
    }


    public void SetCriticalDamage(int value)
    {
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.criticalDamage);
    }
    public void SetCriticalDamage(string value)
    {
        Instance.SetCriticalDamage(int.Parse(value));
    }
    public void AddCriticalDamage(int value)
    {
        value += Instance.graphData.aditionalStatus.criticalDamage;
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.criticalDamage);
    }

    /// <summary>
    /// アディショナルステータスが追加可能かどうかを返す
    /// </summary>
    /// <param name="addition">いくつ追加するか</param>
    /// <returns>可能か不可能か</returns>
    public bool CheckAdditionableStatus(int addition)
    {
        return aditionableStatusCount + addition <= CheckMaxAdditionableStatus();
    }

    /// <summary>
    /// ノード数から追加可能なステータスの最大値を返す
    /// </summary>
    /// <returns>追加可能なアディショナルステータス</returns>
    public int CheckMaxAdditionableStatus(int Nodecount)
    {
        for (int i = 0; maxNodesForPoints.Length > i; ++i)
        {
            if (Nodecount > maxNodesForPoints[i])
            {
                return i;
            }
        }
        return maxNodesForPoints.Length;
    }
    public int CheckMaxAdditionableStatus()
    {
        return CheckMaxAdditionableStatus(Instance.nodeUIs.Count);
    }

    /// <summary>
    /// アディショナルステータスを調整
    /// </summary>
    /// <param name="status">調整するアディショナルステータス</param>
    /// <returns>最終的なアディショナルステータス数</returns>
    public int AjustAdditionalStatus(Status status)
    {
        int max = CheckMaxAdditionableStatus();
        int sum = status.hp + status.attack + status.attackCooltime + status.criticalChance + status.criticalDamage;
        if (sum <= max)
        {
            return sum;
        }

        for (int i = sum - max; i > 0; --i)
        {
            if (status.hp > 0)
            {
                status.hp -= 1;
                continue;
            }
            else if (status.attack > 0)
            {
                status.attack -= 1;
                continue;
            }
            else if (status.attackCooltime > 0)
            {
                status.attackCooltime -= 1;
                continue;
            }
            else if (status.criticalChance > 0)
            {
                status.criticalChance -= 1;
                continue;
            }
            else if (status.criticalDamage > 0)
            {
                status.criticalDamage -= 1;
                continue;
            }
        }
        return sum - max;
    }

    public void ChangeSkin(int skinID)
    {
        Instance.graphData.skin = skinID;
    }

    #endregion


    static public void ConectPorts(PortUI from, PortUI to)
    {
        if (from.port.isToPort && !to.port.isToPort)
        {
            (from, to) = (to, from);
        }
        if (!GraphEditorManager.Instance.CheckConectable(from, to))
        {
            Debug.Log(" 接 続 不 可 能 ");
            return;
        }
        if (from.port.outputConections == null)
        {
            from.port.outputConections = new List<(Node, string)>();
        }

        from.port.outputConections.Add((to.port.owner, to.port.portName));
        if (from.port.owner is LinkedNode fromLinkedNode)
        {
            Array.Resize(ref fromLinkedNode.fromNodes, fromLinkedNode.fromNodes.Length + 1);
            fromLinkedNode.fromNodes[fromLinkedNode.fromNodes.Length - 1] = to.port.owner;
        }
        if (to.port.owner is LinkedNode linkedNode)
        {
            Array.Resize(ref linkedNode.toNodes, linkedNode.toNodes.Length + 1);
            linkedNode.toNodes[linkedNode.toNodes.Length - 1] = from.port.owner;
        }

        GraphEditorManager.Instance.SetLine(from, to);
        Debug.Log(" 接 続 完 了 ");
    }

    public bool CheckConectable(PortUI from, PortUI to)
    {
        if (from == null || to == null)
        {
            return false;
        }
        if (from.owner.node.id == to.owner.node.id)
        {
            return false;
        }
        if (from.port.isToPort || !to.port.isToPort)
        {
            return false;
        }
        if (from.port.isExecutionPort ^ to.port.isExecutionPort)
        {
            return false;
        }
        if (from.port.portType != to.port.portType || from.port.portType.GetType() != to.port.portType.GetType())
        {
            return false;
        }
        return true;
    }

    private void SetLine(PortUI from, PortUI to)
    {
        GameObject conectionObj = new GameObject();
        ConectionUI conection = conectionObj.AddComponent<ConectionUI>();
        conection.fromPort = from;
        conection.toPort = to;
        conection.name = $"from_{from.port.portName}_to_{to.port.portName}";
        conection.name = $"from{from.name}OF{from.owner.node.id}to{to.name}OF{to.owner.node.id}";
        conection.transform.parent = from.transform;
        if (conection.lineRenderer == null)
        {
            conection.lineRenderer = conectionObj.AddComponent<LineRenderer>();
        }
        LineRenderer line = conection.lineRenderer;
        line.positionCount = 2;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = GraphEditorManager.Instance.lineWidth;
        line.endWidth = GraphEditorManager.Instance.lineWidth;
        line.startColor = Color.HSVToRGB(((float)from.portTypeHue) / 360, portUISaturation, portUIValue);
        line.endColor = Color.HSVToRGB(((float)to.portTypeHue) / 360, portUISaturation, portUIValue);
        line.SetPosition(0, from.portPosition.position);
        line.SetPosition(1, to.portPosition.position);
        line.gameObject.layer = LayerMask.NameToLayer(Instance.editorLayer);
        from.outputLines.Add(line);
        from.outputConectionsUI.Add(conection);
    }

    public void DisconectPort(PortUI target)
    {
        if (!target.port.isToPort)
        {
            foreach (var line in target.outputLines)
            {
                Destroy(line.gameObject);
            }
            target.DisconectFromThis();
        }
        else
        {
            foreach (var node in Instance.nodeUIs)
            {
                foreach (var portUI in node.outputPorts)
                {
                    portUI.DisconectWithPortUI(target);
                }
            }
        }
    }


    public async Task SaveGraph(string path, string graphName, string author)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();


        HashSet<string> usedNodeIds = new HashSet<string>() { };
        Instance.graphData.nodes.Clear();
        foreach (var nodeUI in GraphEditorManager.Instance.nodeUIs)
        {
            if (usedNodeIds.Contains(nodeUI.node.id))
            {
                continue;
            }
            if (stopwatch.ElapsedMilliseconds > 3)
            {
                stopwatch.Restart();
                await Task.Yield();
            }
            nodeUI.node.position = nodeUI.transform.position;
            NodeData nodeData = GenerateNodeData(nodeUI);
            Instance.graphData.nodes.Add(nodeData);
            usedNodeIds.Add(nodeUI.node.id);

            if (nodeUI.node is LinkedNode linkedNode)
            {
                List<string> inputIDs = new List<string>();
                List<string> outputIDs = new List<string>();
                foreach (var inputNode in linkedNode.toNodes)
                {
                    inputIDs.Add(inputNode.id);
                }
                foreach (var outputNode in linkedNode.fromNodes)
                {
                    outputIDs.Add(outputNode.id);
                }
                LinkedNodeData linkedNodeData = new LinkedNodeData(linkedNode.id, inputIDs, outputIDs);
                Instance.graphData.linkedNodes.Add(linkedNodeData);
            }
        }
        Instance.AjustAdditionalStatus(Instance.graphData.aditionalStatus);
        Instance.graphData.createdDate = DateTime.Now;
        if (Instance.graphData.graphName.Contains(nameSpacer))
        {
            Instance.graphData.graphName = graphName + nameSpacer + Instance.graphData.graphName;
        }
        else
        {
            Instance.graphData.graphName += graphName;
        }
        if (Instance.graphData.author.Contains(nameSpacer))
        {
            Instance.graphData.author = author + nameSpacer + Instance.graphData.author;
        }
        else
        {
            Instance.graphData.author += author;
        }

        Instance.graphData.version = GraphData.currentVersion;
        string json = JsonUtility.ToJson(Instance.graphData, true);
        EnsureDirectoryExists(path);

        File.WriteAllText(path, json, System.Text.Encoding.UTF8);
    }
    public void SaveGraph(string graphName, string author)
    {
        string parent = Application.persistentDataPath.Replace("/", "\\");

        SaveGraph($"{parent}\\{defaultPath}\\{SanitizeFileName(graphName)}-{SanitizeFileName(author)}__{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.acjson", graphName, author);
    }
    public void SaveGraph(string path)
    {
        Debug.Log("save enemy =========");
        SaveGraph(path, $"_DEFAULT_{System.Guid.NewGuid().ToString()}", $"_DEFAULT_{System.Guid.NewGuid().ToString()}");
    }

    public void SaveGraph()
    {
        SaveGraph("PlaeyreData.json");
    }
    public void SaveEnemyGraph()
    {
        SaveGraph(enemyDataFileName);
    }
    public void SaveGraphDefaultPath(bool isPlayer = true)
    {
        string parent = Application.persistentDataPath.Replace("/", "\\");
        if (isPlayer)
        {
            SaveGraph($"{parent}\\{defaultPath}\\{playerDataFileName}");
        }
        else
        {
            SaveGraph($"{parent}\\{enemyPath}\\{enemyDataFileName}");
        }
    }

    public async void SaveWithDialog()
    {
        string path = await loader.OpenFileDialog();
        if (path == GraphEditorLoader.chanceledMessage)
        {
            return;
        }
        if (string.IsNullOrEmpty(path))
        {
            string parent = Application.persistentDataPath.Replace("/", "\\");
            path =
                $"{parent}\\{defaultPath}\\{playerDataFileName}";
        }
        if (!path.EndsWith(".acjson") && !path.EndsWith(".json") && !path.Contains("."))
        {
            path += ".acjson";
        }
        SaveGraph(path);
    }

    public static string SanitizeFileName(string name)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
        {
            name = name.Replace(c, '_');
        }
        name = name.Replace('/', '_').Replace('\\', '_');
        return name;
    }
    private void EnsureDirectoryExists(string path)
    {
        return;
        path = Path.GetDirectoryName(path);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// ノードUIからノードデータを生成する
    /// </summary>
    /// <param name="nodeUI"></param>
    /// <returns></returns>
    public NodeData GenerateNodeData(NodeUI nodeUI)
    {
        Node node = nodeUI.node;
        NodeData nodeData = new NodeData(node.id, node.nodeType, nodeUI.transform.position, new() { });
        if (node.outputPorts == null || node.outputPorts.Length == 0)
        {
            return nodeData;
        }

        for (int i = 0; i < node.outputPorts.Length; ++i)
        {
            nodeData.outputConnections.Add(new PortConections(node.outputPorts[i].portName, new() { }));

            if (node.outputPorts[i].outputConections == null || node.outputPorts[i].outputConections.Count == 0)
            {
                continue;
            }

            for (int j = 0; j < node.outputPorts[i].outputConections.Count; ++j)
            {
                nodeData.outputConnections[i].toPortNodes.Add(
                    new PortOfNode(
                        node.outputPorts[i].outputConections[j].node.id,
                        node.outputPorts[i].outputConections[j].portName
                    )
                );
            }
        }
        for (int i = 0; i < node.inputValues.Count; ++i)
        {
            if (node.inputValues[i] == null)
            {
                continue;
            }
            if (node.inputValues[i].value.GetType() != typeof(float))
            {
                continue;
            }
            if (nodeData.inputValues == null)
            {
                nodeData.inputValues = new List<InputValue<float>>() { };
            }
            nodeData.inputValues.Add(new InputValue<float>(node.inputValues[i].toPortName, (float)node.inputValues[i].value, isUserset: true));
        }
        Debug.Log("ノードデータ生成完了");
        return nodeData;
    }

    #endregion

    private void Awake()
    {
        Time.timeScale = 1f;
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        if (Instance == null)
        {
            Instance = this;
        }
        Instance.graphData = new GraphData()
        {
            nodes = new List<NodeData>(),
        };
        AddNode(NodeType.Start);
        UpdateAdditionalStatus();
        loader = new GraphEditorLoader();
    }

    private void OnEnable()
    {
        playerInput.actions["Delete"].performed += DeleteNode;
    }
}

[Serializable]
public class NodePrefab
{
    public NodeType type;
    public GameObject prefab;
}


public interface IInteractable
{
    void Delete();
}