using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public abstract class Node
{
    public string id;
    public string nodeType;
    public Vector2 position;

    [NonSerialized] public List<NodePort> inputPorts;
    [NonSerialized] public List<NodePort> outputPorts;

    [NonSerialized] public Dictionary<string, List<Node>> connectedOutputs;
    [NonSerialized] public Dictionary<string, object> inputData;

    public abstract void Initialize();
    public abstract void Execute(GraphExecutor executor);

}
