using System.Collections.Generic;
using UnityEngine;
public class PortUI : MonoBehaviour
{
    [SerializeField]
    public Port port;
    [SerializeField]
    public NodeUI owner;
    [SerializeField]
    public PortTypeHue portTypeHue;
    [SerializeField]
    public Transform portPosition;
    public List<LineRenderer> outputLines;

    public enum PortTypeHue
    {
        OTHER = 262,
        DECIMAL = 0,
        VECTOR = 40,
        EXECUTION = 60,
    }

    public void OnClick()
    {
        if (GraphEditorManager.isSelected == false)
        {
            Debug.Log("port�I��");
            GraphEditorManager.selectedPort = this;
            GraphEditorManager.isSelected = true;
        }
        else
        {
            Debug.Log("port�ڑ�");
            GraphEditorManager.ConectPorts(GraphEditorManager.selectedPort, this);
            GraphEditorManager.isSelected = false;
            GraphEditorManager.selectedPort = null;
        }
    }

    public void Start()
    {
        if (port.owner == null)
        {
            port.owner = owner.node;
        }
    }
}
