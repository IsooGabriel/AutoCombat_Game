using System;
using System.Collections.Generic;
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
    public virtual void EditorInitialize()
    {
        Initialize();
        id = Guid.NewGuid().ToString();
    }
    public abstract void Execute(GraphExecutor executor);
}
