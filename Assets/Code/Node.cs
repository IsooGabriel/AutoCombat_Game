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

    public string executePortName = "Execute";

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

    /// <summary>
    /// ìØÉtÉåÅ[ÉÄÇ≈QueueÇ…ì¸Ç¡ÇƒÇ¢ÇΩèÍçáÇÃÇ›é¿çsÇ≥ÇÍÇÈ
    /// </summary>
    public virtual void FinaryFlame()
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
                inputValues.Add(new InputValue<object>(value.toPortName, (object)value.value, value.isUserset));
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
    public virtual bool TryGetInputValuesWithPort<T>(string toPortName, out List<T> value)
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
            if(value.Count <= 0)
            {
                continue;
            }
            found = true;
        }
        return found;
    }
    public virtual bool TryGetInputValueWithPort<T>(string toPortName, out T value)
    {
        if (TryGetInputValuesWithPort<T>(toPortName, out List<T> values))
        {
            value = values[0];
            return true;
        }
        value = default;
        return false;
    }
    public virtual bool TryGetInputValueWithPort<T>(Port toPort, out List<T> value)
    {
        return TryGetInputValuesWithPort<T>(toPort.portName, out value);
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
    protected Type GetConnectionType()
    {
        Type connectionType = typeof(float);
        HashSet<Node> visitedNodes = new HashSet<Node>();
        foreach (var fromPort in outputPorts)
        {
            foreach (var toPort in fromPort.outputConections)
            {
                if (visitedNodes.Contains(toPort.node))
                {
                    continue;
                }
                visitedNodes.Add(toPort.node);
                foreach (var inputPort in toPort.node.inputPorts)
                {
                    if (inputPort.portName != toPort.portName)
                    {
                        continue;
                    }
                    if (inputPort.portType == typeof(Vector2))
                    {
                        return typeof(Vector2);
                    }
                    if (inputPort.portType == typeof(float))
                    {
                        connectionType = typeof(float);
                    }
                    else if (inputPort.portType == typeof(bool))
                    {
                        connectionType = typeof(bool);
                    }
                    connectionType = inputPort.portType;
                    break;
                }
            }
        }
        return connectionType;
    }
}
