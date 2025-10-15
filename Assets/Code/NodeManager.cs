using System.Collections.Generic;
using System.Linq;

public class NodeManager
{
    private List<INode> nodes;
    private NodeManager()
    {
        nodes = new List<INode>();
    }
    public void AddNode(INode node)
    {
        nodes.Add(node);
    }
    public void RemoveNode(INode node)
    {
        nodes.Remove(node);
    }
    public T GetNode<T>() where T : class, INode
    {
        return nodes.OfType<T>().FirstOrDefault();
    }
    public List<INode> GetAllNodes()
    {
        return new List<INode>(nodes);
    }

    public bool Run()
    {
        return true;
    }
}