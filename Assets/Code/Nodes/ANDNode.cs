using System;
using System.Collections.Generic;
using UnityEngine;

public class ANDNode : LinkedNode
{
    const string elementPortName = "element";
    const string resultPortName = "result";

    private Type connectionType = typeof(float);
    private bool firstExecution = true;
    private int executionCount = 0;

    public override void Initialize()
    {
        useLimit = -1;
        nodeType = NodeType.AND;
        inputPorts = new Port[]
        {
            new Port("Execute", typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
            new Port(elementPortName, typeof(object), isRequired:false, isInput:true, isExecutionPort:false, this),
        };
        outputPorts = new Port[]
        {
            new Port("Execute", typeof(bool), false, false, true, this),
            new Port(resultPortName, typeof(object), false, false, false, this),
        };
    }

    public override void Execute(GraphExecutor executor)
    {
        if (firstExecution)
        {
            connectionType = GetConnectionType();
            firstExecution = false;
        }
        executionCount++;
        if (executionCount < inputNodes.Length)
        {
            return;
        }
        TryGetInputValueWithPort(elementPortName, out List<object> values);
        switch (connectionType)
        {
            case Type t when t == typeof(float):
                executor.SendData(this, resultPortName, AndFloat(values));
                break;
            case Type t when t == typeof(bool):
                executor.SendData(this, resultPortName, AndBool(values));
                break;
            case Type t when t == typeof(Vector2):
                executor.SendData(this, resultPortName, AndVector2(values));
                break;
            default:
                throw new Exception("Unsupported connection type in ANDNode");
        }
        executor.EnqueueConnected(this, outputPorts[0].portName);
    }

    public override void FlameInitialize()
    {
        executionCount = 0;
    }
    private float AndFloat(List<object> values)
    {
        double result = 0f;
        foreach (var value in values)
        {
            if (value is float f)
            {
                result += (float)f;
            }
            else if (value is bool b)
            {
                result += b ? 1f : 0f;
            }
            else if (value is Vector2 v)
            {
                result += v.x + v.y;
            }
            else if (value is string s)
            {
                if (float.TryParse(s, out float parsedValue))
                {
                    result += parsedValue;
                }
            }
        }
        return (float)result;
    }

    private bool AndBool(List<object> values)
    {
        foreach (var value in values)
        {
            if (value is bool b)
            {
                if (b)
                {
                    return true;
                }
            }
            else if (value is float f)
            {
                if (f != 0f)
                {
                    return true;
                }
            }
            else if (value is Vector2 v)
            {
                if (v.x != 0f || v.y != 0f)
                {
                    return true;
                }
            }
            else if (value is string s)
            {
                if (float.TryParse(s, out float parsedValue))
                {
                    if (parsedValue != 0f)
                    {
                        return true;
                    }
                }
            }
        }
        return true;
    }

    private Vector2 AndVector2(List<object> values)
    {
        Vector2 result = new Vector2(0, 0);
        foreach (var value in values)
        {
            if (value is Vector2 v)
            {
                result += v;
            }
            else if (value is float f)
            {
                result += new Vector2(f, 0);
            }
            else if (value is bool b)
            {
                result += b ? new Vector2(1, 1) : new Vector2(0, 0);
            }
            else if (value is string s)
            {
                if (float.TryParse(s, out float parsedValue))
                {
                    result += new Vector2(parsedValue, parsedValue);
                }
            }
        }
        return result;
    }
    private Type GetConnectionType()
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
