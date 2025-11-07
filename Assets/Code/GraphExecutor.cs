using System.Collections.Generic;
using System.Linq;

public class GraphExecutor
{
    public Character myCharacter;
    public Character enemy;
    private Dictionary<string, Node> nodes = new Dictionary<string, Node> { };
    private Queue<Node> executionQueue;


    #region public関数


    public GraphExecutor(GraphData graphData, Character myCharacter, Character enemy)
    {
        this.myCharacter = myCharacter;
        this.enemy = enemy;
        LoadGraph(graphData);
    }

    /// <summary>
    /// 1tick分実行
    /// </summary>
    public void ExecuteTick()
    {
        executionQueue = new Queue<Node>();
        var executed = new HashSet<string>();
        Node startNode = nodes.Values.FirstOrDefault(n => n.nodeType == NodeType.Start);
        if (startNode != null)
        {
            executionQueue.Enqueue(startNode);
        }

        while (executionQueue.Count > 0)
        {
            Node node = executionQueue.Dequeue();
            if (!executed.Contains(node.id))
            {
                node.useCount = 1;
                executed.Add(node.id);
            }
            else if (node.useCount >= node.useLimit)
            {
                continue;
            }
            else
            {
                node.useCount++;
            }

            // 必須ポートが満たされているか確認
            if (!CheckExecutable(node) && node.nodeType != NodeType.Start)
            {
                continue;
            }

            node.Execute(this);
        }
        foreach (var node in executed)
        {
            nodes[node].inputData.Clear();
        }
    }

    /// <summary>
    /// ノードが次のノードを実行キューに追加
    /// </summary>
    /// <param name="node">ポートの親</param>
    /// <param name="portName">発火させるポート名</param>
    public void EnqueueConnected(Node node, string portName)
    {
        foreach (var port in node.outputPorts)
        {
            if (port.name != portName)
            {
                continue;
            }
            foreach (var connected in port.outputConections)
            {
                executionQueue.Enqueue(connected.Item1);
            }
        }
    }

    /// <summary>
    /// ノードがデータを次のノードに送る
    /// </summary>
    /// <param name="node">送信元ノード</param>
    /// <param name="portName">送信元ポート名</param>
    /// <param name="data">送るデータ</param>
    public bool SendData(Node node, string portName, object data)
    {
        bool isSent = false;
        foreach (var outputPort in node.outputPorts)
        {
            if (outputPort.name != portName)
            {
                continue;
            }
            foreach (var inputPort in outputPort.outputConections)
            {
                if (inputPort.node == null)
                {
                    continue;
                }
                if (inputPort.node.inputData == null)
                {
                    inputPort.node.inputData = new Dictionary<string, List<object>>();
                }

                if (inputPort.node.inputData.ContainsKey(inputPort.portName))
                {
                    if (inputPort.node.inputData[inputPort.portName] != null)
                    {
                        continue;
                    }
                }
                else
                {
                    inputPort.node.inputData.Add(inputPort.portName, new() { data });
                    continue;
                }

                inputPort.node.inputData[inputPort.portName].Add(data);

                isSent = true;
            }
        }
        return isSent;
    }


    #endregion

    #region private関数


    private void LoadGraph(GraphData graphData)
    {
        nodes.Clear();

        foreach (var nodeData in graphData.nodes)
        {
            var node = NodeFactory.Create(nodeData.type);
            node.Initialize();
            node.id = nodeData.id;
            node.position = nodeData.position;
            nodes[node.id] = node;
        }

        // 接続を構築
        foreach (var nodeData in graphData.nodes)
        {
            Node node = nodes[nodeData.id];
            if (nodeData.outputConnections == null || nodeData.outputConnections.Count == 0)
            {
                continue;
            }
            foreach (var output in node.outputPorts)
            {
                output.outputConections.Clear();
            }

            foreach (var portConections in nodeData.outputConnections)
            {
                Port myPort = node.outputPorts.FirstOrDefault(port => port.name == portConections.fromPortName);
                if (myPort == null)
                {
                    continue;
                }
                foreach (var portNode in portConections.toPortNodes)
                {
                    Node targetNode = nodes.FirstOrDefault(node => node.Key == portNode.nodeId).Value;
                    if (targetNode == null)
                    {
                        continue;
                    }
                    myPort.outputConections.Add((targetNode, portNode.portName));
                }
            }
        }
        myCharacter.aditionalStatus = graphData.aditionalStatus;
    }

    private bool CheckExecutable(Node node)
    {
        foreach (var port in node.inputPorts)
        {
            if (port == null || !port.isRequired)
            {
                continue;
            }
            if (!node.inputData.ContainsKey(port.name))
            {
                return false;
            }
        }
        return true;
    }

    #endregion
}

public static class NodeFactory
{
    private static Dictionary<NodeType, System.Func<Node>> _creators = new()
    {
        { NodeType.Start, () => new StartNode() },
        { NodeType.Move, () => new MoveNode() },
        //{ NodeType.Attack, () => new AttackNode() },
        //{ NodeType.Jump, () => new JumpNode() },
        //{ NodeType.GetDistance, () => new GetDistanceNode() },
        //{ NodeType.GetHP, () => new GetHPNode() },
        //{ NodeType.Compare, () => new CompareNode() },
        //{ NodeType.Branch, () => new BranchNode() },
        { NodeType.DEBUG, () => new _DebugNode() },
    };

    public static Node Create(NodeType type)
    {
        if (_creators.TryGetValue(type, out var creator))
        {
            return creator();
        }

        throw new System.ArgumentException($"Unknown node type: {type}");
    }

    // エディタで使うノードタイプ一覧(Startを除外)
    public static IEnumerable<NodeType> GetCreatableNodeTypes()
    {
        return System.Enum.GetValues(typeof(NodeType))
            .Cast<NodeType>()
            .Where(t => t != NodeType.Start && t != NodeType.DEBUG);
    }
}