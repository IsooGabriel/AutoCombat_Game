
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public interface INode
{
    IEnumerable<Port> InputPorts { get; }
    IEnumerable<Port> OutputPorts { get; }
    void Execute(NodeContext context);
    static public List<Port> GetPorts<T>(IEnumerable<Port> ports)
    {
        List<Port> result = new List<Port>();
        foreach (var port in ports)
        {
            if(!(port.DataType is T))
            {
                continue;
            }
            result.Add(port);
        }
        return result;
    }
}
