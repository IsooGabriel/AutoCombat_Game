using UnityEngine;
public class NodeUI : MonoBehaviour
{
    public Node node = new _DebugNode();
    [SerializeField]
    public PortUI[] inputPorts;
    [SerializeField]
    public PortUI[] outputPorts;

    public virtual void Awake()
    {
        node.id = System.Guid.NewGuid().ToString();
    }
}
