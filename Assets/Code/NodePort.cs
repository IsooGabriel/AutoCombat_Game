using System;

[Serializable]
public class NodePort
{
    public string name;
    public Type type;
    public bool isRequired;
    public bool isInput;
    public bool isExecutionPort;
    public Node owner;

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