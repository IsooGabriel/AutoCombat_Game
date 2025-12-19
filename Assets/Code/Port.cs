using System;
using System.Collections.Generic;

[Serializable]
public class Port

{
    public string portName;
    public Type portType;
    public bool isRequired;
    public bool isInput;
    public bool isExecutionPort;
    public Node owner;
    public List<(Node node, string portName)> outputConections = new() { };

    public List<(Node, string)> GetConections()
    {
        return outputConections;
    }
    public List<(Node, string)> GetConections(string nodeID)
    {
        List<(Node, string)> connections = new List<(Node, string)>();
        foreach (var connection in outputConections)
        {
            if (connection.Item1.id == nodeID)
            {
                connections.Add(connection);
            }
        }
        return connections;
    }
    public List<(Node, string)> GetConections(Node node)
    {
        return GetConections(node.id);
    }
    public Port(string name, Type type, bool isRequired, bool isInput, bool isExecutionPort, Node owner)
    {
        this.portName = name;
        this.portType = type;
        this.isRequired = isRequired;
        this.isInput = isInput;
        this.isExecutionPort = isExecutionPort;
        this.owner = owner;
    }
}