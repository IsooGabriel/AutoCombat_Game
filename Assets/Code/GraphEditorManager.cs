using System.Runtime.CompilerServices;
using TMPro;

public class GraphEditorManager
{
    static public GraphEditorManager Instance = new GraphEditorManager();
    static public bool isSelected = false;
    static public PortUI selectedPort = null;

    static public void ConectPorts(PortUI from, PortUI to)
    {
        if(!GraphEditorManager.Instance.IsConectable(from, to))
        {
            return;
        }

    }

    public bool IsConectable(PortUI from, PortUI to)
    {
        if (from == null || to == null)
        {
            return false;
        }
        if (from.owner.node.id == to.owner.node.id)
        {
            return false;
        }
        if (from.port.isInput || !to.port.isInput)
        {
            return false ;
        }
        if (from.port.type != to.port.type)
        {
            return false;
        }
        return true;
    }
}
