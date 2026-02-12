using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;

public class GraphEditorCustumer : MonoBehaviour
{
    [SerializeField]
    public CustumSettings[] custums;
    void Start()
    {
        foreach (var custum in custums)
        {
            custum.SelectActive(StageSelector.sceneName);
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
        _enables.ForEach(element => element?.SetActive(isActive));
        _disables.ForEach(element => element?.SetActive(!isActive));
    }
}
