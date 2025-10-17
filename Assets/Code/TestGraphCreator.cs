using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using System;
public class TestGraphCreator : MonoBehaviour
{
    void Start()
    {
        // GraphData() のコンストラクタで自動的にStartノードが作られる
        var graph = new GraphData();

        // Startノードは既に存在するので、そこから接続を追加
        var startNode = graph.nodes[0]; // 必ず存在

        // 保存
        string json = JsonUtility.ToJson(graph, true);
        System.IO.File.WriteAllText(Application.dataPath + $"/TestGraph{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.json", json);
        Debug.Log("Graph created with Start node!"+ Application.dataPath);
    }
}