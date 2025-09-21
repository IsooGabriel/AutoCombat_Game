using System;
using System.Collections.Generic;

public enum PortDirection
{
    Input,
    Output
}

public class Port
{
    public string Name { get; }
    public Type DataType { get; }       // int, float, bool ‚È‚Ç
    public PortDirection Direction { get; }
    public INode Owner { get; }         // Š‘®ƒm[ƒh

    private List<Port> _connections = new();

    public IEnumerable<Port> Connections => _connections;

    public Port(string name, Type dataType, PortDirection direction, INode owner)
    {
        Name = name;
        DataType = dataType;
        Direction = direction;
        Owner = owner;
    }

    public void Connect(Port other)
    {
        if (other.DataType != DataType)
        {
            throw new InvalidOperationException("Œ^‚ªˆê’v‚µ‚Ü‚¹‚ñ");
        }
        if (other.Direction == Direction)
        {
            throw new InvalidOperationException("“üo—Í‚Ì•ûŒü‚ª“¯‚¶‚Å‚·");
        }
        _connections.Add(other);
    }
}
