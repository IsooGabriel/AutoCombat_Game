using UnityEngine;

public class NodeUI : MonoBehaviour
{
    [SerializeField]
    public Node node = new _DebugNode();
    [SerializeField]
    public PortUI[] inputPorts;
    [SerializeField]
    public PortUI[] outputPorts;

    public void Awake()
    {
        node.id = System.Guid.NewGuid().ToString();
    }
}
