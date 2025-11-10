using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class GraphEditorManager : MonoBehaviour
{
    static public GraphEditorManager Instance = null;
    public bool isSelected = false;
    public PortUI selectedPort = null;
    private float lineWidth = 0.05f;
    public List<NodeUI> nodeUIs = new List<NodeUI>();
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
    }
    private void SetPortUIsPort(PortUI[] portUIs, Port[] ports)
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


    #region 追加ステータス

    public void UpdateAdditionalStatus()
    {
        additionalHP.text = Instance.graphData.aditionalStatus.hp.ToString();
        additionalAttack.text = $"{Instance.graphData.aditionalStatus.attack}";
        additionalAttackCT.text = $"{Instance.graphData.aditionalStatus.attackCooltime}";
        additionalCriticalChance.text = $"{Instance.graphData.aditionalStatus.criticalChance}";
        additionalCriticalDamage.text = $"{Instance.graphData.aditionalStatus.criticalDamage}";
    }

    public void AddHP(int value)
    {
        Instance.graphData.aditionalStatus.hp += value;
    }
    public void SetHP(int value)
    {
        Instance.graphData.aditionalStatus.hp = value;
    }
    public void SetHP(string value)
    {
        Instance.SetHP(int.Parse(value));
    }
    public void AddAttack(int value)
    {
        Instance.graphData.aditionalStatus.attack += value;
    }
    public void SetAttack(int value)
    {
        Instance.graphData.aditionalStatus.attack = value;
    }
    public void SetAttack(string value)
    {
        Instance.SetAttack(int.Parse(value));
    }
    public void AddAttackCT(int value)
    {
        Instance.graphData.aditionalStatus.attackCooltime += value;
    }
    public void SetAttackCT(int value)
    {
        Instance.graphData.aditionalStatus.attackCooltime = value;
    }
    public void SetAttackCT(string value)
    {
        Instance.SetAttackCT(int.Parse(value));
    }
    public void AddCriticalChance(int value)
    {
        Instance.graphData.aditionalStatus.criticalChance += value;
    }
    public void SetCriticalChance(int value)
    {
        Instance.graphData.aditionalStatus.criticalChance = value;
    }
    public void SetCriticalChance(string value)
    {
        Instance.SetCriticalChance(int.Parse(value));
    }
    public void AddCriticalDamage(int value)
    {
        Instance.graphData.aditionalStatus.criticalDamage += value;
    }
    public void SetCriticalDamage(int value)
    {
        Instance.graphData.aditionalStatus.criticalDamage = value;
    }
    public void SetCriticalDamage(string value)
    {
        Instance.SetCriticalDamage(int.Parse(value));
    }

    #endregion


    static public void ConectPorts(PortUI from, PortUI to)
    {
        if (from.port.isInput && !to.port.isInput)
        {
            (from, to) = (to, from);
        }
        if (!GraphEditorManager.Instance.CheckConectable(from, to))
        {
            return;
        }
        if (from.port.outputConections == null)
        {
            from.port.outputConections = new List<(Node, string)>();
        }
        from.port.outputConections.Add((to.port.owner, to.port.name));

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
        if (from.port.isInput || !to.port.isInput)
        {
            return false;
        }
        if (from.port.type != to.port.type)
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
        conection.name = $"from_{from.port.name}_to_{to.port.name}";
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
        from.outputLines.Add(line);
    }
    public void SaveGraph()
    {
        HashSet<string> usedNodeIds = new HashSet<string>();
        foreach (var nodeUI in GraphEditorManager.Instance.nodeUIs)
        {
            if (usedNodeIds.Contains(nodeUI.node.id))
            {
                continue;
            }
            nodeUI.node.position = nodeUI.transform.position;
            NodeData nodeData = GenerateNodeData(nodeUI);
            Instance.graphData.nodes.Add(nodeData);
            usedNodeIds.Add(nodeUI.node.id);
        }
        Instance.graphData.aditionalStatus.hp = 100;
        string json = JsonUtility.ToJson(Instance.graphData, true);
        System.IO.File.WriteAllText(Application.dataPath + $"/Jsons/TestGraph{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.json", json);
        Debug.Log("グラフ保存完了");
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
            nodeData.outputConnections.Add(new PortConections(node.outputPorts[i].name, new() { }));

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
        Debug.Log("ノードデータ生成完了");
        return nodeData;
    }

    #endregion

    private void Awake()
    {
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
    }
}

[Serializable]
public class NodePrefab
{
    public NodeType type;
    public GameObject prefab;
}
