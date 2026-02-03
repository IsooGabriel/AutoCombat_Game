using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
public class PortUI : MonoBehaviour
{
    [NonSerialized]
    public Port port;
    [SerializeField]
    public NodeUI owner;
    [SerializeField]
    public PortTypeHue portTypeHue;
    [SerializeField, Header("ポート名")]
    public TextMeshProUGUI tmpUGUI;
    [SerializeField]
    public Transform portPosition;
    [SerializeField]
    public Image portImage;
    [NonSerialized]
    public List<LineRenderer> outputLines = new();
    [NonSerialized]
    public List<ConectionUI> outputConectionsUI = new();

    public enum PortTypeHue
    {
        OTHER = 262,
        DECIMAL = 50,
        VECTOR = 326,
        BOOLEAN = 190,
        EXECUTION = 130,
    }

    public void OnClick()
    {
        if (GraphEditorManager.Instance.isSelected == false)
        {
            Debug.Log("port選択");
            GraphEditorManager.Instance.selectedPort = this;
            GraphEditorManager.Instance.isSelected = true;
            portImage.color = Color.HSVToRGB((float)portTypeHue / 360, GraphEditorManager.Instance.portUISerectSaturation, GraphEditorManager.Instance.portUISerectValue);
        }
        else
        {
            GraphEditorManager.Instance.selectedPort.portImage.color = Color.HSVToRGB(((float)GraphEditorManager.Instance.selectedPort.portTypeHue) / 360, GraphEditorManager.Instance.portUISaturation, GraphEditorManager.Instance.portUIValue);
            portImage.color = Color.HSVToRGB(((float)portTypeHue) / 360, GraphEditorManager.Instance.portUISaturation, GraphEditorManager.Instance.portUIValue);
            Debug.Log("port接続");
            GraphEditorManager.ConectPorts(GraphEditorManager.Instance.selectedPort, this);
            GraphEditorManager.Instance.isSelected = false;
            GraphEditorManager.Instance.selectedPort = null;
        }
    }

    public void Disconect(PortUI target)
    {
        outputLines.RemoveAll(l => l.gameObject.GetComponent<ConectionUI>().toPort == target);
        for(int i = 0; outputConectionsUI.Count > i; ++i)
        {
            var conn = outputConectionsUI[i];
            if (conn.toPort == target)
            {
                outputConectionsUI.RemoveAt(i);
                Destroy(conn.gameObject);
                break;
            }
        }
        port.outputConections.RemoveAll(c => c.portName == target.port.portName && c.node == target.port.owner);
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
        if (owner != null && port != null)
        {
            port.owner = owner.node;
        }
        if (tmpUGUI != null)
        {
            tmpUGUI.text = port.portName;
        }
    }
}
