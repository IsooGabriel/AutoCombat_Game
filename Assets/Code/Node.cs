using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public abstract class Node
{
    public string id;
    public NodeType nodeType;
    public Vector2 position;
    public int useLimit = 10;
    public int useCount = 0;

    [NonSerialized] public NodePort[] inputPorts;
    [NonSerialized] public NodePort[] outputPorts;

    [NonSerialized] public Dictionary<string, List<Node>> connectedOutputs;
    [NonSerialized] public Dictionary<string, object> inputData;

    public abstract void Initialize();
    public abstract void Execute(GraphExecutor executor);
}
