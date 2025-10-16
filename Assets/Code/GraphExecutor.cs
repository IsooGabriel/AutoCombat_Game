using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;

public class GraphExecutor
{
    public CharacterController character;
    private Dictionary<string, Node> nodes;
    private Queue<Node> executionQueue;
    private HashSet<string> executedThisTick; // 同一ノードの重複実行防止

    public GraphExecutor(GraphData graphData, CharacterController character)
    {
        this.character = character;
        LoadGraph(graphData);
    }

    private void LoadGraph(GraphData graphData)
    {
        nodes = new Dictionary<string, Node>();

        // ノードの生成
        foreach (var nodeData in graphData.nodes)
        {
            Node node = CreateNodeFromType(nodeData.type);
            node.id = nodeData.id;
            node.position = nodeData.position;
            node.Initialize();

            // 定数入力値を設定
            if (nodeData.inputValues != null)
            {
                // TODO: リフレクションで設定
            }

            nodes[node.id] = node;
        }

        // 接続を構築
        foreach (var nodeData in graphData.nodes)
        {
            Node node = nodes[nodeData.id];
            node.connectedOutputs = new Dictionary<string, List<Node>>();

            if (nodeData.outputConnections != null)
            {
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
    }

    private Node CreateNodeFromType(string type)
    {
        // リフレクションで動的生成
        Type nodeType = Type.GetType(type + "Node");
        return (Node)Activator.CreateInstance(nodeType);
    }

    // 1tick分実行
    public void ExecuteTick()
    {
        executionQueue = new Queue<Node>();
        executedThisTick = new HashSet<string>();

        Node startNode = nodes.Values.FirstOrDefault(n => n.nodeType == "Start");
        if (startNode != null)
        {
            executionQueue.Enqueue(startNode);
        }

        while (executionQueue.Count > 0)
        {
            Node node = executionQueue.Dequeue();

            // 重複実行防止
            if (executedThisTick.Contains(node.id))
            {
                continue;
            }

            // 必須ポートが満たされているか確認
            if (!CanExecute(node))
            {
                continue;
            }

            executedThisTick.Add(node.id);
            node.Execute(this);

            // データをクリア(次tickのため)
            node.inputData.Clear();
        }
    }

    private bool CanExecute(Node node)
    {
        foreach (var port in node.inputPorts.Where(p => p.isRequired))
        {
            if (!node.inputData.ContainsKey(port.name))
            {
                return false;
            }
        }
        return true;
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

    // ノードがデータを次のノードに送る
    public void SendData(Node node, string portName, object data)
    {
        if (node.connectedOutputs.ContainsKey(portName))
        {
            foreach (var connected in node.connectedOutputs[portName])
            {
                // 対応する入力ポート名を見つける(簡略化)
                var inputPort = connected.inputPorts.FirstOrDefault(p => p.type == GetPortType(portName, node));
                if (inputPort != null)
                {
                    connected.inputData[inputPort.name] = data;
                    executionQueue.Enqueue(connected);
                }
            }
        }
    }

    private PortType GetPortType(string portName, Node node)
    {
        return node.outputPorts.First(p => p.name == portName).type;
    }
}