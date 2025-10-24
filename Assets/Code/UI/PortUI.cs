using UnityEngine;

public class PortUI : MonoBehaviour
{
    [SerializeField]
    public Port port;
    [SerializeField]
    public NodeUI owner;
    [SerializeField]
    public PortTypeHue portTypeHue;

    public LineRenderer[] outputLines;

    public enum PortTypeHue
    {
        OTHER = 262,
        DECIMAL = 0,
        VECTOR = 40,
        EXECUTION = 60,
    }

    public void OnClick()
    {
        if(GraphEditorManager.isSelected == false)
        {
            GraphEditorManager.selectedPort = this;
            GraphEditorManager.isSelected = true;
        }
        else
        {
            GraphEditorManager.ConectPorts(GraphEditorManager.selectedPort, this);
            GraphEditorManager.isSelected = false;
            GraphEditorManager.selectedPort = null;
        }
    }
}
