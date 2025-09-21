
using System.Collections.Generic;

public interface INode
{
    IEnumerable<Port> InputPorts { get; }
    IEnumerable<Port> OutputPorts { get; }
    void Execute(NodeContext context);
}
