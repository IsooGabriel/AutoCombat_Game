using UnityEngine;
public class GraphEditorManager
{
    static public GraphEditorManager Instance = new GraphEditorManager();
    static public bool isSelected = false;
    static public PortUI selectedPort = null;

    static public void ConectPorts(PortUI from, PortUI to)
    {
        if (!GraphEditorManager.Instance.IsConectable(from, to))
        {
            return;
        }
        if(from.port.outputConections == null)
        {
            from.port.outputConections = new System.Collections.Generic.List<(Node, string)>();
        }
        from.port.outputConections.Add((to.port.owner, to.port.name));
        LineRenderer line = new LineRenderer();
        line.positionCount = 2;
        line.SetPosition(0, from.transform.position);
        line.SetPosition(1, to.transform.position);
        from.outputLines.Add(line);
        Debug.Log(" ê⁄ ë± äÆ óπ ");
    }

    public bool IsConectable(PortUI from, PortUI to)
    {
        Debug.Log("ê⁄ë±â¬î\Ç©ämîFíÜ...");
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
            return false;
        }
        if (from.port.type != to.port.type)
        {
            return false;
        }
        return true;
    }
}
