using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Port
{
    public string portName; // ポートの内部的な識別名
    public string portNameJP = defaultPortName; // ポートの日本語表示名
    public const string defaultPortName = "デフォルト (x_x;"; // 日本語名が未設定の場合のデフォルト文字列
    [SerializeField] private string portTypeName; // シリアライズ可能な型名
    public Type portType 
    { 
        get => string.IsNullOrEmpty(portTypeName) ? typeof(float) : Type.GetType(portTypeName);
        set => portTypeName = value?.AssemblyQualifiedName;
    }
    public bool isRequired; // このポートへの接続が必須かどうか
    public bool isToPort; // 入力ポート（接続を受ける側）かどうか
    public bool isExecutionPort; // 実行フローを制御する実行ポートかどうか
    public Node owner; // このポートを所有しているノード
    public List<(Node node, string portName)> outputConections = new() { }; // このポートから出力される接続のリスト

    /// <summary>
    /// このポートに紐付けられているすべての接続情報を取得します。
    /// </summary>
    /// <returns>接続先ノードとポート名のペアのリスト</returns>
    public List<(Node, string)> GetConections()
    {
        return outputConections;
    }

    /// <summary>
    /// 指定されたノードIDに紐付けられている接続情報を取得します。
    /// </summary>
    /// <param name="nodeID">検索対象のノードID</param>
    /// <returns>条件に一致する接続先ノードとポート名のペアのリスト</returns>
    public List<(Node, string)> GetConections(string nodeID)
    {
        List<(Node, string)> connections = new List<(Node, string)>();
        foreach (var connection in outputConections)
        {
            if (connection.Item1.id == nodeID)
            {
                connections.Add(connection);
            }
        }
        return connections;
    }

    /// <summary>
    /// 指定されたノードに紐付けられている接続情報を取得します。
    /// </summary>
    /// <param name="node">検索対象のノードインスタンス</param>
    /// <returns>条件に一致する接続先ノードとポート名のペアのリスト</returns>
    public List<(Node, string)> GetConections(Node node)
    {
        return GetConections(node.id);
    }

    /// <summary>
    /// ポートの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="name">ポートの名前</param>
    /// <param name="type">ポートが扱う型</param>
    /// <param name="isRequired">接続が必須か</param>
    /// <param name="isInput">入力ポートか（trueなら入力、falseなら出力）</param>
    /// <param name="isExecutionPort">実行ポートか</param>
    /// <param name="owner">所有者となるノード</param>
    public Port(string name, Type type, bool isRequired, bool isInput, bool isExecutionPort, Node owner)
    {
        this.portName = name;
        this.portType = type;
        this.isRequired = isRequired;
        this.isToPort = isInput;
        this.isExecutionPort = isExecutionPort;
        this.owner = owner;

        if (owner != null && owner.nameToJP.ContainsKey(portName))
        {
            portNameJP = owner.nameToJP[portName];
        }
    }
}
