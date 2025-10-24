using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class GraphData
{
    const string version = "0.1";
    public List<NodeData> nodes = new();
    const string startNodeId = "START_NODE";
    public string graphName = "New AI";
    public string author = "";
    public DateTime createdDate = DateTime.Now;

    public GraphData()
    {
        nodes.Add(new NodeData()
        {
            id = startNodeId,
            type = NodeType.Start,
            position = new Vector2(10, 0),
            outputConnections = new()
            {
                new PortConections("exec", new (){} ),
            }
        });
    }

    public PortConections GetConection(string portName, string nodeID = startNodeId)
    {
        foreach (var node in nodes)
        {
            if (node.id != nodeID)
            {
                continue;
            }
            foreach (var conection in node.outputConnections)
            {
                if (conection.fromPortName == portName)
                {
                    return conection;
                }
            }
        }
        return null;
    }
}

[Serializable]
public class NodeData
{
    public string id;
    public NodeType type;
    public Vector2 position;
    public List<PortConections> outputConnections = new();
    public Dictionary<string, object> inputValues = new();
}
[Serializable]
public class PortConections
{
    public string fromPortName;
    public List<PortOfNode> toPortNodes = new();
    public PortConections(string fromPortName, List<PortOfNode> toPortNode)
    {
        this.fromPortName = fromPortName;
        this.toPortNodes = toPortNode;
    }

    /// <summary>
    /// 接続先ノードのID一覧を取得
    /// </summary>
    /// <returns></returns>
    public string[] GetNodeIDs()
    {
        List<string> nodeIDs = new() { };
        foreach (var portOfNode in toPortNodes)
        {
            nodeIDs.Add(portOfNode.nodeId);
        }
        return nodeIDs.ToArray();
    }
}
[Serializable]
public class PortOfNode
{
    public string nodeId;
    public string portName;
    public PortOfNode(string nodeId, string portName)
    {
        this.nodeId = nodeId;
        this.portName = portName;
    }
}
public enum NodeType
{
    Start,
    Move,
    Attack,
    Jump,
    GetDistance,
    GetHP,
    Compare,
    Branch,
    DEBUG
}