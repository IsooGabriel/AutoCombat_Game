using UnityEngine;
using System.Collections.Generic;
using System;

public class GraphEditorCustumer : MonoBehaviour
{
    [SerializeField]
    public CustumSettings[] custums;
    [SerializeField]
    private ButtonToNode[] nodeButtons = { };
    [SerializeField]
    private NodeSerializeWhithStage[] nodeSerialize = { };
    void Start()
    {
        foreach (var custum in custums)
        {
            custum.SelectActive(StageSelector.sceneName);
        }
        foreach (var node in nodeSerialize)
        {
            node.SetSetting(nodeButtons);
        }
    }
}

[Serializable]
public class CustumSettings
{
    [SerializeField]
    private string _stageName = "Game";
    [SerializeField]
    private List<GameObject> _enables = new();
    [SerializeField]
    private List<GameObject> _disables = new();

    public void SelectActive(string stageName)
    {
        SelectActive(stageName == _stageName);
    }
    public void SelectActive(bool isActive)
    {
        _disables.ForEach(element => element?.SetActive(!isActive));
        _enables.ForEach(element => element?.SetActive(isActive));
    }
}

[Serializable]
public class ButtonToNode
{
    [SerializeField]
    public NodeType nodeType;
    [SerializeField]
    public GameObject button;
}

[Serializable]
public class NodeSerializeWhithStage
{
    [SerializeField]
    private string _stageName = "Game";
    [SerializeField]
    private NodeSerializeSetting[] _nodeTypes;

    public void SetSetting(ButtonToNode[] buttons)
    {
        if (StageSelector.sceneName != _stageName)
        {
            return;
        }
        foreach (var node in _nodeTypes)
        {
            node.SetSetting(buttons);
        }
    }
}

[Serializable]
public class NodeSerializeSetting
{
    [SerializeField]
    private NodeType _nodeType;
    [SerializeField]
    private bool isSerialize = false;

    public void SetSetting(ButtonToNode[] buttons)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].nodeType == _nodeType)
            {
                buttons[i].button.SetActive(isSerialize);
            }
        }
    }
}
