using System;
using System.Collections.Generic;
using System.Xml;

public enum PortDirection
{
    Input,
    Output
}

public class Port
{
    public Port(string name, Type dataType, PortDirection direction, INode owner, bool isRequired = false)
    {
        Name = name;
        DataType = dataType;
        Direction = direction;
        Owner = owner;
        IsRequired = isRequired;
    }

    public string Name { get; }
    public Type DataType { get; }
    public PortDirection Direction { get; }
    public INode Owner { get; }
    public bool IsRequired{ get; }

    private List<Port> _connections = new();

    public IEnumerable<Port> Connections => _connections;


    public void Connect(Port other)
    {
        if (other.DataType != DataType)
        {
            throw new InvalidOperationException("å^Ç™àÍívÇµÇ‹ÇπÇÒ");
        }
        if (other.Direction == Direction)
        {
            throw new InvalidOperationException("ì¸èoóÕÇÃï˚å¸Ç™ìØÇ∂Ç≈Ç∑");
        }
        _connections.Add(other);
    }
    public List<Port> GetConnections<T>()
    {
        List<Port> result = new List<Port>();
        foreach (var port in _connections)
        {
            if (!(port.DataType is T))
            {
                continue;
            }
            result.Add(port);
        }
        return result;
    }
    public bool Output<T>(T value)
    {
        foreach (var output in GetConnections<T>())
        {
            NodeContext context = new();
            context.SetValue(output, value);
            output.Owner.Execute(context);
        }
        return true;
    }
}
