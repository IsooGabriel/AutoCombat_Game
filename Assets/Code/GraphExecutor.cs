using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        Node startNode = nodes.Values.FirstOrDefault(n => n.nodeType == "Start");
        if (startNode != null)
        {
            executionQueue.Enqueue(startNode);
        }

        while (executionQueue.Count > 0)
        {
            Node node = executionQueue.Dequeue();

            // 重複実行防止
            if (node.useLimit == 0)
            {
                continue;
            }

            // 必須ポートが満たされているか確認
            if (!CanExecute(node))
            {
                continue;
            }

            node.useLimit--;
            node.Execute(this);

            // データをクリア(次tickのため)
            node.inputData.Clear();
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
    private Type GetPortType(string portName, Node node)
    {
        return node.outputPorts.First(p => p.name == portName).type;
    }


    #endregion
}