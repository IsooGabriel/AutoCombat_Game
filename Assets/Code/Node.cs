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

    [NonSerialized] public List<InputValue> inputData = new() { };

    public abstract void Initialize();
    public abstract void Execute(GraphExecutor executor);
    public virtual void EditorInitialize()
    {
        Initialize();
        id = Guid.NewGuid().ToString();
    }
    public virtual void SetData(NodeData data)
    {
        id = data.id;
        position = data.position;
        nodeType = data.type;
        inputData = data.inputValues ?? new List<InputValue>();
    }

    public virtual bool InputValueContainsPort(string toPortName)
    {
        foreach (var data in inputData)
        {
            if (data.toPortName == toPortName)
            {
                return true;
            }
        }
        return false;
    }
    public virtual bool TryGetInputValueWithPort<T>(string toPortName, out List<T> value)
    {
        value = new List<T>();
        if (InputValueContainsPort(toPortName) == false)
        {
            return false;
        }
        bool found = false;
        foreach (var data in inputData)
        {
            if (data.toPortName != toPortName)
            {
                continue;
            }
            value.Add((T)data.value);
            found = true;
        }
        return found;
    }
    public virtual bool TryGetInputvalueWithPort<T>(Port toPort, out List<T> value)
    {
        return TryGetInputValueWithPort<T>(toPort.name, out value);
    }
    public virtual bool TryGetInputValueWithType<T>(out List<T> value)
    {
        value = new List<T>();
        bool found = false;
        foreach (var data in inputData)
        {
            if (data.value is T typedValue)
            {
                value.Add(typedValue);
                found = true;
            }
        }
        return found;
    }
}
