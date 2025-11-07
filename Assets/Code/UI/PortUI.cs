using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField]
    public Image portImage;
    public List<LineRenderer> outputLines;

    public enum PortTypeHue
    {
        OTHER = 262,
        DECIMAL = 0,
        VECTOR = 326,
        EXECUTION = 130,
    }

    public void OnClick()
    {
        if (GraphEditorManager.Instance.isSelected == false)
        {
            Debug.Log("portëIë");
            GraphEditorManager.Instance.selectedPort = this;
            GraphEditorManager.Instance.isSelected = true;
            portImage.color = Color.HSVToRGB((float)portTypeHue / 360, GraphEditorManager.Instance.portUISerectSaturation, GraphEditorManager.Instance.portUISerectValue);
        }
        else
        {
            GraphEditorManager.Instance.selectedPort.portImage.color = Color.HSVToRGB(((float)GraphEditorManager.Instance.selectedPort.portTypeHue) / 360, GraphEditorManager.Instance.portUISaturation, GraphEditorManager.Instance.portUIValue);
            portImage.color = Color.HSVToRGB(((float)portTypeHue) / 360, GraphEditorManager.Instance.portUISaturation, GraphEditorManager.Instance.portUIValue);
            Debug.Log("portê⁄ë±");
            GraphEditorManager.ConectPorts(GraphEditorManager.Instance.selectedPort, this);
            GraphEditorManager.Instance.isSelected = false;
            GraphEditorManager.Instance.selectedPort = null;
        }
    }

    public void Start()
    {
        if (portImage != null)
        {
            portImage.color = Color.HSVToRGB(((float)portTypeHue) / 360, GraphEditorManager.Instance.portUISaturation, GraphEditorManager.Instance.portUIValue);
        }
        if (port.owner == null)
        {
            port.owner = owner.node;
        }
    }
}
