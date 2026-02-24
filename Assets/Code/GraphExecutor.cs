using System.Collections.Generic;
using System.Linq;

public class GraphExecutor
{
    public Character myCharacter;
    public Character enemy;
    private Dictionary<string, Node> nodes = new Dictionary<string, Node> { };
    private Dictionary<string, LinkedNode> linkedNodes = new Dictionary<string, LinkedNode> { };
    public Queue<Node> executionQueue;
    public Node startNode = null;
    public HashSet<string> startedNodes = new HashSet<string> { };

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
        if (startNode != null)
        {
            executionQueue.Enqueue(startNode);
        }

        while (executionQueue.Count > 0)
        {
            Node node = executionQueue.Dequeue();
            if (!startedNodes.Contains(node.id))
            {
                node.StartInitialize();
                startedNodes.Add(node.id);
            }
            if (!executed.Contains(node.id))
            {
                node.useCount = 1;
                node.FlameInitialize();
                executed.Add(node.id);
            }
            else if (node.useLimit < 0)
            {
                node.useCount++;
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
            nodes[node].FinaryFlame();
            List<InputValue<object>> usersets = nodes[node].inputValues.Where(v => v.isUserset).ToList();
            nodes[node].inputValues = usersets;
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
            if (port.portName != portName)
            {
                continue;
            }
            foreach (var connected in port.outputConections)
            {
                SendData(node, node.executePortName, (bool)true);
                executionQueue.Enqueue(connected.Item1);
            }
        }
    }

    /// <summary>
    /// ノードがデータを次のノードに送る
    /// </summary>
    /// <param name="fromNode">送信元ノード</param>
    /// <param name="fromPortName">送信元ポート名</param>
    /// <param name="data">送るデータ</param>
    /// <returns>送信成功数が1以上かのbool、要するに送信できたかの判定</returns>
    public bool SendData(Node fromNode, string fromPortName, object data)
    {
        bool isSent = false;
        foreach (var fromPort in fromNode.outputPorts)
        {
            if (fromPort.portName != fromPortName)
            {
                continue;
            }
            foreach (var toPort in fromPort.outputConections)
            {
                if (toPort.node == null)
                {
                    continue;
                }
                if (toPort.node.inputValues == null)
                {
                    toPort.node.inputValues = new() { };
                }

                toPort.node.inputValues.Add(new InputValue<object>(toPort.portName, (object)data));

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
        linkedNodes.Clear();


        foreach (var nodeData in graphData.nodes)
        {
            var newNode = NodeFactory.Create(nodeData.type);
            newNode.Initialize();
            newNode.SetData(nodeData);
            nodes[newNode.id] = newNode;
            if (newNode.nodeType == NodeType.Start && startNode == null)
            {
                startNode = newNode;
            }
        }

        Node node = null;
        // 接続を構築
        foreach (var nodeData in graphData.nodes)
        {
            node = nodes[nodeData.id];
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
                Port myPort = node.outputPorts.FirstOrDefault(port => port.portName == portConections.fromPortName);
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

        LinkedNode linkedNode = null;
        foreach (var linkedNodeData in graphData.linkedNodes)
        {
            UnityEngine.Debug.Log(linkedNodeData.id + nodes.ContainsKey(linkedNodeData.id) + linkedNodes.ContainsKey(linkedNodeData.id));
            if (nodes.ContainsKey(linkedNodeData.id) && nodes[linkedNodeData.id] is LinkedNode lNode)
            {
                linkedNodes[linkedNodeData.id] = lNode;
            }
            else
            {
                continue;
            }
            linkedNode = nodes[linkedNodeData.id] as LinkedNode;

            List<Node> inputNodes = new List<Node> { };
            List<Node> outputNodes = new List<Node> { };
            foreach (var inputID in linkedNodeData.inputNodeIDs)
            {
                if (!nodes.ContainsKey(inputID))
                {
                    continue;
                }
                inputNodes.Add(nodes[inputID]);
            }
            foreach (var outputID in linkedNodeData.outputNodeIDs)
            {
                if (!nodes.ContainsKey(outputID))
                {
                    continue;
                }
                outputNodes.Add(nodes[outputID]);
            }
            linkedNode.toNodes = inputNodes.ToArray();
            linkedNode.fromNodes = outputNodes.ToArray();
            nodes[linkedNodeData.id] = linkedNode;
        }
        myCharacter.aditionalStatus = graphData.aditionalStatus;
    }

    private bool CheckExecutable(Node node)
    {
        foreach (var port in node.inputPorts)
        {
            if (port == null || !port.isRequired || port.isExecutionPort)
            {
                continue;
            }
            if (!node.inputValues.Any(d => d.toPortName == port.portName))
            {
                return false;
            }
        }
        return true;
    }

    #endregion
}
