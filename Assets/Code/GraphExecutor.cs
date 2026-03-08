using System.Collections.Generic;
using System.Linq;

public class GraphExecutor
{
    public Character myCharacter; // このグラフを実行する自身のキャラクター
    public Character enemy; // グラフの対象（敵）となるキャラクター
    private Dictionary<string, Node> nodes = new Dictionary<string, Node> { }; // グラフを構成する全ノードの辞書（IDがキー）
    private Dictionary<string, LinkedNode> linkedNodes = new Dictionary<string, LinkedNode> { }; // リンクされたノード（LinkedNode）の辞書
    public Queue<Node> executionQueue; // 現在のTickで実行待ちのノードキュー
    public Node startNode = null; // グラフの開始地点となるノード
    public HashSet<string> startedNodes = new HashSet<string> { }; // すでに初期化（StartInitialize）済みのノードIDの集合

    #region public関数

    /// <summary>
    /// GraphExecutorのコンストラクタ。グラフデータを読み込み、実行環境を初期化します。
    /// </summary>
    /// <param name="graphData">読み込むグラフのデータ</param>
    /// <param name="myCharacter">実行主体となるキャラクター</param>
    /// <param name="enemy">対象となる敵キャラクター</param>
    public GraphExecutor(GraphData graphData, Character myCharacter, Character enemy)
    {
        this.myCharacter = myCharacter;
        this.enemy = enemy;
        LoadGraph(graphData);
    }

    /// <summary>
    /// 1Tick（フレーム）分のグラフ実行処理を行います。開始ノードから順に接続されたノードを処理します。
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
    /// 指定されたノードの出力ポートに接続されているすべてのノードを実行キューに追加します。
    /// </summary>
    /// <param name="node">接続元となるノード</param>
    /// <param name="portName">発火させる出力ポートの名前</param>
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
                SendData(node, Node.executePortName, (bool)true);
                executionQueue.Enqueue(connected.Item1);
            }
        }
    }

    /// <summary>
    /// ノードから接続先のすべての入力ポートへデータを送信します。
    /// </summary>
    /// <param name="fromNode">送信元となるノード</param>
    /// <param name="fromPortName">送信元となる出力ポートの名前</param>
    /// <param name="data">送信するデータオブジェクト</param>
    /// <returns>少なくとも1つのポートに送信できた場合はtrue、それ以外はfalse</returns>
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

    /// <summary>
    /// シリアライズされたGraphDataから実行用のノード構成と接続関係を構築します。
    /// </summary>
    /// <param name="graphData">読み込むソースデータ</param>
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

    /// <summary>
    /// ノードの実行が可能かどうか（必須入力ポートがすべて接続されているか）をチェックします。
    /// </summary>
    /// <param name="node">チェック対象のノード</param>
    /// <returns>実行可能な場合はtrue、不足がある場合はfalse</returns>
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
