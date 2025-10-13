
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public interface INode
{
    IEnumerable<Port> InputPorts { get; }
    IEnumerable<Port> OutputPorts { get; }
    void Execute(NodeContext context);
    public Port GetPort(string name)
    {
        return InputPorts.FirstOrDefault(p => p.Name == name);
    }
    public Port GetPort<T>() where T : Port
    {
        return InputPorts.OfType<T>().FirstOrDefault();
    }
}
