using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class GraphData
{
    const string version = "0.1";
    public List<NodeData> nodes = new();
    public string startNodeId = "START_NODE";
    public string graphName = "New AI";
    public string author = "";
    public DateTime createdDate = DateTime.Now;

    public GraphData()
    {
        nodes.Add(new NodeData()
        {
            id = startNodeId,
            type = NodeType.Start,
            position = new Vector2(10, 0)
        });
    }
}

[Serializable]
public class NodeData
{
    public string id;
    public NodeType type;
    public Vector2 position;
    public Dictionary<string, List<string>> outputConnections = new();
    public Dictionary<string, object> inputValues = new();
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