using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GraphEditorManager : MonoBehaviour
{
    static public GraphEditorManager Instance = null;
    static public bool isSelected = false;
    static public PortUI selectedPort = null;
    private float lineWidth = 0.05f;
    public List<NodeUI> nodes = new List<NodeUI>();
    public GameObject nodesParent;
    public NodePrefab[] nodePrefabs;

    public GraphData graphData;

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

        Instance.nodes.Add(nodeUI);
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
        line.startColor = Color.HSVToRGB((int)to.portTypeHue, 90f, 80f);
        line.endColor = Color.HSVToRGB((int)to.portTypeHue, 90f, 80f);
        line.SetPosition(0, from.portPosition.position);
        line.SetPosition(1, to.portPosition.position);
        from.outputLines.Add(line);
    }
    public void SaveGraph()
    {
        HashSet<string> usedNodeIds = new HashSet<string>();
        foreach (var nodeUI in GraphEditorManager.Instance.nodes)
        {
            if (usedNodeIds.Contains(nodeUI.node.id))
            {
                continue;
            }
            NodeData nodeData = GenerateNodeData(nodeUI);
            Instance.graphData.nodes.Add(nodeData);
            usedNodeIds.Add(nodeUI.node.id);
        }
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
        NodeData nodeData = new NodeData()
        {
            id = node.id,
            type = node.nodeType,
            position = nodeUI.transform.position
        };
        nodeData.outputConnections = new() { };
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
    }
}

[Serializable]
public class NodePrefab
{
    public NodeType type;
    public GameObject prefab;
}
