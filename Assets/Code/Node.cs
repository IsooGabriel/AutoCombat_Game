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

    [NonSerialized] public Port[] inputPorts = new Port[] { };
    [NonSerialized] public Port[] outputPorts = new Port[] { };

    [NonSerialized] public Dictionary<string, object> inputData = new Dictionary<string, object> { };

    public abstract void Initialize();
    public abstract void Execute(GraphExecutor executor);
}

public class BaseNode:Node
{
    public override void Initialize()
    {
        id = Guid.NewGuid().ToString();
    }
    public override void Execute(GraphExecutor executor)
    {
    }
}