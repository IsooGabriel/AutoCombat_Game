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

    [NonSerialized] public List<InputValue<object>> inputValues = new() { };

    public abstract void Initialize();
    public abstract void Execute(GraphExecutor executor);
    public virtual void EditorInitialize()
    {
        Initialize();
        id = Guid.NewGuid().ToString();
    }
    public virtual void StartInitialize()
    {
        return;
    }
    public virtual void FlameInitialize()
    {
        return;
    }
    public virtual void SetData(NodeData data)
    {
        id = data.id;
        position = data.position;
        nodeType = data.type;
        if (data.inputValues == null)
        {
            data.inputValues = new List<InputValue<float>>() { };
        }
        else if (data.inputValues.Count > 0)
        {
            foreach (var value in data.inputValues)
            {
                inputValues.Add(new InputValue<object>(value.toPortName, (object)value.value));
            }
        }
    }

    public virtual bool InputValueContainsPort(string toPortName)
    {
        foreach (var data in inputValues)
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
        foreach (var data in inputValues)
        {
            if (data.toPortName != toPortName)
            {
                continue;
            }
            if (data.value is T setValue)
            {
                value.Add((T)setValue);
            }
            else if (data.value is List<T> listSetValue)
            {
                value.Add((T)listSetValue[0]);
            }
            //îGâGêFÇ
            found = true;
        }
        return found;
    }
    public virtual bool TryGetInputValueWithPort<T>(Port toPort, out List<T> value)
    {
        return TryGetInputValueWithPort<T>(toPort.name, out value);
    }
    public virtual bool TryGetInputValueWithType<T>(out List<T> value)
    {
        value = new List<T>();
        bool found = false;
        foreach (var data in inputValues)
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
