using System.Collections.Generic;
using System;
using UnityEngine;
[Serializable]
public class GraphData
{
    public string version = "1.0";
    public List<NodeData> nodes = new();
    public string graphName = "New AI";
    public string author = "";
    public DateTime createdDate = DateTime.Now;
}

[Serializable]
public class NodeData
{
    public string id;
    public string type;
    public Vector2 position;
    public Dictionary<string, List<string>> outputConnections = new();
    public Dictionary<string, object> inputValues = new();
}