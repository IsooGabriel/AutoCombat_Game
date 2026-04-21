using System;
using System.Collections.Generic;
using UnityEngine;
using Weapon;
[Serializable]
public class GraphData
{
    public const string currentVersion = "0.3";
    public string version = currentVersion;
    public List<NodeData> nodes = new();
    public List<LinkedNodeData> linkedNodes = new();
    const string startNodeId = "START_NODE";
    public string graphName = "New AI";
    public string author = "";
    public DateTime createdDate = DateTime.Now;
    public Status aditionalStatus = new() { hp = 0, attack = 0, attackCooltime = 0, criticalChance = 0, criticalDamage = 0 };
    public int weapon = 0;
    public int skin = 0;
    public const int noWeapon = -1;

    public GraphData()
    {
        nodes.Add(
            new NodeData(
                startNodeId,
                NodeType.Start,
                new Vector2(10, 0),
                new() { new PortConections("exec", new() { }) }
            )
        );
    }

    public PortConections GetConection(string portName, string nodeID = startNodeId)
    {
        foreach (var node in nodes)
        {
            if (node.id != nodeID)
            {
                continue;
            }
            foreach (var conection in node.outputConnections)
            {
                if (conection.fromPortName == portName)
                {
                    return conection;
                }
            }
        }
        return null;
    }
}

[Serializable]
public class NodeData
{
    public NodeData(string id, NodeType type, Vector2 position, List<PortConections> outputConnection, List<InputValue<SerializedValue>> inpuValues = null)
    {
        this.id = id;
        this.type = type;
        this.position = position;
        this.outputConnections = outputConnection;
        this.inputValues = inpuValues;
    }
    public string id;
    public NodeType type;
    public Vector2 position;
    public List<PortConections> outputConnections = new();
    public List<InputValue<SerializedValue>> inputValues = new();
}
[Serializable]
public class LinkedNodeData
{
    public string id;
    public List<string> inputNodeIDs = new();
    public List<string> outputNodeIDs = new();
    public LinkedNodeData(string id, List<string> inputNodeIDs, List<string> outputNodeIDs)
    {
        this.id = id;
        this.inputNodeIDs = inputNodeIDs;
        this.outputNodeIDs = outputNodeIDs;
    }
}
[Serializable]
public class PortConections
{
    public string fromPortName;
    public List<PortOfNode> toPortNodes = new();
    public PortConections(string fromPortName, List<PortOfNode> toPortNode)
    {
        this.fromPortName = fromPortName;
        this.toPortNodes = toPortNode;
    }

    /// <summary>
    /// 接続先ノードのID一覧を取得
    /// </summary>
    /// <returns></returns>
    public string[] GetNodeIDs()
    {
        List<string> nodeIDs = new() { };
        foreach (var portOfNode in toPortNodes)
        {
            nodeIDs.Add(portOfNode.nodeId);
        }
        return nodeIDs.ToArray();
    }
}
[Serializable]
public class PortOfNode
{
    public string nodeId;
    public string portName;
    public PortOfNode(string nodeId, string portName)
    {
        this.nodeId = nodeId;
        this.portName = portName;
    }
}
[Serializable]
public class InputValue<T>
{
    public string toPortName;
    public T value;
    public bool isUserset = false;

    public InputValue(string toPortName, T value, bool isUserset = false)
    {
        this.toPortName = toPortName;
        this.value = value;
        this.isUserset = isUserset;
    }
}

/// <summary>
/// JSON文字列と型情報を保持し、自由な型をシリアライズするためのクラス
/// </summary>
[Serializable]
public class SerializedValue
{
    public string valueJson;
    public string typeName;

    public SerializedValue(object value)
    {
        if (value == null)
        {
            return;
        }
        this.typeName = value.GetType().AssemblyQualifiedName;
        
        // プリミティブ型や文字列はそのまま文字列として、それ以外はJSONとして保存
        if (value.GetType().IsPrimitive || value is string)
        {
            this.valueJson = value.ToString();
        }
        else
        {
            this.valueJson = JsonUtility.ToJson(value);
        }
    }

    /// <summary>
    /// 指定されたターゲット型への変換を試みつつ、オブジェクトを復元します。
    /// </summary>
    public object ToObject(Type targetType = null)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return null;
        }
        Type originalType = Type.GetType(typeName);
        if (originalType == null)
        {
            return null;
        }

        object result = null;
        try
        {
            if (originalType.IsPrimitive || originalType == typeof(string))
            {
                result = Convert.ChangeType(valueJson, originalType);
            }
            else
            {
                result = JsonUtility.FromJson(valueJson, originalType);
            }
        }
        catch
        {
            return null;
        }

        // int <-> float などの柔軟な型変換
        if (targetType != null && targetType != originalType && result != null)
        {
            try
            {
                return Convert.ChangeType(result, targetType);
            }
            catch
            {
                return result;
            }
        }
        return result;
    }
}
public enum NodeType
{
    Start = 0,
    Move = 1,
    Attack,
    Jump,
    GetDistance,
    GetHP,
    Compare,
    Branch,
    DEBUG,
    SetValue,
    SetVector,
    AND,
    IF,
    BreakVector,
    GetPosition,
    SetBool,
    DoNTime,
    Sum,
    GetStatus,
    Multiply,
    Accele,
    Stop, 
}
