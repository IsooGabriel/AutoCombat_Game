using UnityEngine;
public class NodeUI : MonoBehaviour
{
    public Node node = new _DebugNode();
    [SerializeField]
    public PortUI[] inputPorts;
    [SerializeField]
    public PortUI[] outputPorts;

    public void SetOwnerInPorts(PortUI[] ports)
    {
        foreach (var portUI in ports)
        {
            if (portUI == null || portUI.port == null)
            {
                continue;
            }
            portUI.owner = this;
            portUI.port.owner = node;
        }
    }

    public virtual void Awake()
    {
        node.id = System.Guid.NewGuid().ToString();
        if (inputPorts != null && inputPorts.Length > 0)
        {
            SetOwnerInPorts(inputPorts);
        }
        if (outputPorts != null && outputPorts.Length > 0)
        {
            SetOwnerInPorts(outputPorts);
        }
    }
}
