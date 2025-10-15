
using System.Collections.Generic;
using System.Linq;

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
            if (!(port.DataType is T))
            {
                continue;
            }
            result.Add(port);
        }
        return result;
    }
    static public bool CheckExecutable(IEnumerable<Port> ports, NodeContext context)
    {
        bool hasInput = false;
        foreach (var port in ports)
        {
            if (port.isRequired)
            {
                if (port.Connections == null || port.Connections.Count() == 0)
                {
                    return false;
                }
            }
            if (port.Connections.Count() == 0)
            {
                continue;
            }
            foreach (var connectedPort in port.Connections)
            {
                if (context.CheckRegister(connectedPort))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            hasInput = true;
        }
        return hasInput;
    }

}
