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
                new PortConection("exec", new (){} ),
            }
        });
    }

    public PortConection GetConection(string portName, string nodeID = startNodeId)
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
    public List<PortConection> outputConnections = new();
    public Dictionary<string, object> inputValues = new();
}
[Serializable]
public class PortConection
{
    public string fromPortName;
    public List<PortOfNode> toPortNode = new();
    public PortConection(string fromPortName, List<PortOfNode> toPortNode)
    {
        this.fromPortName = fromPortName;
        this.toPortNode = toPortNode;
    }
    public string[] GetNodesID()
    {
        List<string> nodeIDs = new() { };
        foreach (var portOfNode in toPortNode)
        {
            nodeIDs.Add(portOfNode.NodeId);
        }
        return nodeIDs.ToArray();
    }
}
[Serializable]
public class PortOfNode
{
    public string NodeId;
    public string PortName;
    public PortOfNode(string nodeId, string portName)
    {
        NodeId = nodeId;
        PortName = portName;
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