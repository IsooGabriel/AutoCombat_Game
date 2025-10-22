using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public abstract class Node
{
    public string id;
    public NodeType nodeType;
    public Vector2 position;
    public int useLimit = 1;
    public int useCount = 0;

    [NonSerialized] public NodePort[] inputPorts = new NodePort[] { };
    [NonSerialized] public NodePort[] outputPorts = new NodePort[] { };

    [NonSerialized] public Dictionary<string, List<Node>> connectedOutputs = new Dictionary<string, List<Node>> { };
    [NonSerialized] public Dictionary<string, object> inputData = new Dictionary<string, object> { };

    public abstract void Initialize();
    public abstract void Execute(GraphExecutor executor);
}
