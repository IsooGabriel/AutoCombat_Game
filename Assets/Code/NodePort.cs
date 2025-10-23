using System;
using System.Collections.Generic;

[Serializable]
public class NodePort
{
    public string name;
    public Type type;
    public bool isRequired;
    public bool isInput;
    public bool isExecutionPort;
    public Node owner;
    public Dictionary<Node,string> outputConections = new () { };

    public NodePort(string name, Type type, bool isRequired, bool isInput, bool isExecutionPort, Node owner)
    {
        this.name = name;
        this.type = type;
        this.isRequired = isRequired;
        this.isInput = isInput;
        this.isExecutionPort = isExecutionPort;
        this.owner = owner;
    }
}