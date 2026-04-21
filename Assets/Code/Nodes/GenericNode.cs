using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 外部定義（JSON）に基づいて動的に構成される汎用ノードクラス
/// </summary>
[Serializable]
public class GenericNode : Node
{
    public string customTypeName;
    [NonSerialized]
    public NodeDefinition definition;

    public GenericNode()
    {
        this.nodeType = NodeType.Custom;
    }

    public GenericNode(NodeDefinition def)
    {
        this.definition = def;
        this.customTypeName = def.typeName;
        this.nodeType = NodeType.Custom;
    }

    public override void Initialize()
    {
        if (definition == null)
        {
            // ロード時などは定義がまだない場合があるため、
            // 外部の管理クラス（NodeRegistryなど）から再取得する仕組みが必要
            definition = CustomNodeRegistry.GetDefinition(customTypeName);
        }

        if (definition == null) return;

        // ポート名の辞書をクリア（実行ポートは残す）
        nameToJP.Clear();
        nameToJP.Add(executePortName, executePortNameJP);

        List<Port> inputs = new List<Port>();
        List<Port> outputs = new List<Port>();

        // 実行ポート
        inputs.Add(new Port(executePortName, typeof(void), false, true, true, this));
        outputs.Add(new Port(executePortName, typeof(void), false, false, true, this));

        foreach (var pDef in definition.inputs)
        {
            if (!nameToJP.ContainsKey(pDef.name)) nameToJP.Add(pDef.name, pDef.name);
            bool isExec = pDef.type.ToLower() == "execution";
            inputs.Add(new Port(pDef.name, StringToType(pDef.type), false, true, isExec, this));
        }

        foreach (var pDef in definition.outputs)
        {
            if (!nameToJP.ContainsKey(pDef.name)) nameToJP.Add(pDef.name, pDef.name);
            bool isExec = pDef.type.ToLower() == "execution";
            outputs.Add(new Port(pDef.name, StringToType(pDef.type), false, false, isExec, this));
        }

        inputPorts = inputs.ToArray();
        outputPorts = outputs.ToArray();
    }

    public override void Execute(GraphExecutor executor)
    {
        // MODノードは今のところ「データを受け渡すだけ」または「次のノードへ流すだけ」
        // 処理を追加したい場合は、ここに共通のロジックや、スクリプト実行エンジン（Lua等）を繋ぐことも可能
        executor.EnqueueConnected(this, executePortName);
    }

    private Type StringToType(string typeStr)
    {
        return typeStr.ToLower() switch
        {
            "float" => typeof(float),
            "bool" => typeof(bool),
            "vector2" => typeof(Vector2),
            "int" => typeof(int),
            "string" => typeof(string),
            _ => typeof(object),
        };
    }

    public override void SetData(NodeData data)
    {
        base.SetData(data);
        this.customTypeName = data.customTypeName;
        // 定義の再紐付け
        this.definition = CustomNodeRegistry.GetDefinition(this.customTypeName);
    }
}

/// <summary>
/// 外部ノード定義を管理する静的レジストリ
/// </summary>
public static class CustomNodeRegistry
{
    private static Dictionary<string, NodeDefinition> _definitions = new();

    public static void Register(NodeDefinition def)
    {
        _definitions[def.typeName] = def;
    }

    public static NodeDefinition GetDefinition(string typeName)
    {
        if (string.IsNullOrEmpty(typeName)) return null;
        _definitions.TryGetValue(typeName, out var def);
        return def;
    }

    public static IEnumerable<NodeDefinition> GetAllDefinitions() => _definitions.Values;

    public static void Clear() => _definitions.Clear();
}
