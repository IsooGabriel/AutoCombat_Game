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
    public Type DataType { get; }       // int, float, bool �Ȃ�
    public PortDirection Direction { get; }
    public INode Owner { get; }         // �����m�[�h

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
            throw new InvalidOperationException("�^����v���܂���");
        }
        if (other.Direction == Direction)
        {
            throw new InvalidOperationException("���o�͂̕����������ł�");
        }
        _connections.Add(other);
    }
}
