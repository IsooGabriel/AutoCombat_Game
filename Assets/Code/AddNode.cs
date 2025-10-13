using System.Collections.Generic;
using UnityEngine;

public class AddNode : MonoBehaviour, INode
{
    public AddNode()
    {
        var inputA = new Port("A", typeof(decimal), PortDirection.Input, this);
        var inputB = new Port("B", typeof(decimal), PortDirection.Input, this);
        var output = new Port("Result", typeof(decimal), PortDirection.Output, this);
        InputPorts = new[] { inputA, inputB };
        OutputPorts = new[] { output };
    }
    public IEnumerable<Port> InputPorts { get; private set; }
    public IEnumerable<Port> OutputPorts { get; private set; }
    public void Execute(NodeContext context)
    {
    }
}