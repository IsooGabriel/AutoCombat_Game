using System;

[Serializable]
public class NodePort
{
    public string name;
    public PortType type;
    public bool isRequired;
    public bool isInput;

    public NodePort(string name, PortType type, bool isRequired, bool isInput)
    {
        this.name = name;
        this.type = type;
        this.isRequired = isRequired;
        this.isInput = isInput;
    }
}

public enum PortType
{
    Execution,
    Float,
    Bool,
    Vector2
}