using System.Collections.Generic;
using System.Linq;

public class GraphExecutor
{
    public CharacterController character;
    private Dictionary<string, Node> nodes;
    private Queue<Node> executionQueue;


    #region public関数


    public GraphExecutor(GraphData graphData, CharacterController character)
    {
        this.character = character;
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
            if (!CanExecute(node))
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
        if (node.connectedOutputs.ContainsKey(portName))
        {
            foreach (var connected in node.connectedOutputs[portName])
            {
                executionQueue.Enqueue(connected);
            }
        }
    }

    /// <summary>
    /// ノードがデータを次のノードに送る
    /// </summary>
    /// <param name="node">送信元ノード</param>
    /// <param name="portName">送信に使うポート名</param>
    /// <param name="data">送るデータ</param>
    public bool SendData(Node node, string portName, object data)
    {
        bool isSent = false;
        if (!node.connectedOutputs.ContainsKey(portName))
        {
            return false;
        }
        foreach (var targetPort in node.outputPorts)
        {
            if (targetPort == null)
            {
                continue;
            }
            Node targetNode = targetPort.owner;
            if (targetNode == null)
            {
                continue;
            }
            if (targetNode.inputData == null || targetNode.inputData.Count <= 0)
            {
                continue;
            }
            if (targetNode.inputData.ContainsKey(targetPort.name))
            {
                if (targetNode.inputData[targetPort.name] != null)
                {
                    continue;
                }
            }
            else
            {
                targetNode.inputData.Add(targetPort.name, data);
                return true;
            }

            targetNode.inputData[targetPort.name] = data;

            isSent = true;
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
            node.id = nodeData.id;
            node.position = nodeData.position;
            node.Initialize();
            nodes[node.id] = node;
        }

        // 接続を構築
        foreach (var nodeData in graphData.nodes)
        {
            Node node = nodes[nodeData.id];
            node.connectedOutputs = new Dictionary<string, List<Node>>();

            if (nodeData.outputConnections == null)
            {
                continue;
            }

            foreach (var kvp in nodeData.outputConnections)
            {
                string portName = kvp.Key;
                List<Node> connected = kvp.Value
                    .Select(id => nodes[id])
                    .ToList();
                node.connectedOutputs[portName] = connected;
            }
        }
    }

    private bool CanExecute(Node node)
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
        //{ NodeType.Move, () => new MoveNode() },
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