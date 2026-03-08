using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Node
{
    public string id; // ノードの一意な識別子
    public NodeType nodeType; // ノードの種類を示す列挙型
    public Vector2 position; // エディタ上での表示位置
    public int useLimit = 1; // ノードの最大実行回数
    public int useCount = 0; // 現在の実行回数

    public const string executePortName = "Execute"; // 実行用ポートの内部名
    public const string executePortNameJP = "実行"; // 実行用ポートの表示名（日本語）

    [NonSerialized] public Port[] inputPorts = new Port[] { }; // 入力ポートの配列
    [NonSerialized] public Port[] outputPorts = new Port[] { }; // 出力ポートの配列

    [NonSerialized] public List<InputValue<object>> inputValues = new() { }; // 入力された値のリスト

    public Dictionary<string, string> nameToJP= new() { {executePortName, executePortNameJP } }; // ポート名と日本語名の対応辞書

    /// <summary>
    /// ノードの初期化処理を行います。
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// ノードの実行処理を行います。
    /// </summary>
    /// <param name="executor">実行を管理するGraphExecutorのインスタンス</param>
    public abstract void Execute(GraphExecutor executor);

    /// <summary>
    /// エディタ上でノードが作成された際の初期化処理を行います。
    /// </summary>
    public virtual void EditorInitialize()
    {
        Initialize();
        id = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// グラフの実行開始時に呼び出される初期化処理です。
    /// </summary>
    public virtual void StartInitialize()
    {
        return;
    }

    /// <summary>
    /// 毎フレームの実行開始時に呼び出される初期化処理です。
    /// </summary>
    public virtual void FlameInitialize()
    {
        return;
    }

    /// <summary>
    /// 最終フレームにおいて、Queueにノードが残っている場合に実行される処理です。
    /// </summary>
    public virtual void FinaryFlame()
    {
        return;
    }

    /// <summary>
    /// NodeDataからノードの情報を設定します。
    /// </summary>
    /// <param name="data">設定元となるシリアライズされたノードデータ</param>
    public virtual void SetData(NodeData data)
    {
        id = data.id;
        position = data.position;
        nodeType = data.type;
        if (data.inputValues == null)
        {
            data.inputValues = new List<InputValue<float>>() { };
        }
        else if (data.inputValues.Count > 0)
        {
            foreach (var value in data.inputValues)
            {
                inputValues.Add(new InputValue<object>(value.toPortName, (object)value.value, value.isUserset));
            }
        }
    }

    /// <summary>
    /// 指定したポート名の入力値が含まれているかを確認します。
    /// </summary>
    /// <param name="toPortName">確認するポートの名前</param>
    /// <returns>含まれている場合はtrue、そうでない場合はfalse</returns>
    public virtual bool InputValueContainsPort(string toPortName)
    {
        foreach (var data in inputValues)
        {
            if (data.toPortName == toPortName)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 指定したポート名に接続された複数の入力値を取得します。
    /// </summary>
    /// <typeparam name="T">取得する値の型</typeparam>
    /// <param name="toPortName">取得先のポート名</param>
    /// <param name="value">取得された値のリスト</param>
    /// <returns>値が見つかった場合はtrue、そうでない場合はfalse</returns>
    public virtual bool TryGetInputValuesWithPort<T>(string toPortName, out List<T> value)
    {
        value = new List<T>();
        if (InputValueContainsPort(toPortName) == false)
        {
            return false;
        }
        bool found = false;
        foreach (var data in inputValues)
        {
            if (data.toPortName != toPortName)
            {
                continue;
            }
            if (data.value is T setValue)
            {
                value.Add((T)setValue);
            }
            else if (data.value is List<T> listSetValue)
            {
                value.Add((T)listSetValue[0]);
            }
            if(value.Count <= 0)
            {
                continue;
            }
            found = true;
        }
        return found;
    }

    /// <summary>
    /// 指定したポート名に接続された単一の入力値を取得します。
    /// </summary>
    /// <typeparam name="T">取得する値の型</typeparam>
    /// <param name="toPortName">取得先のポート名</param>
    /// <param name="value">取得された値</param>
    /// <returns>値が見つかった場合はtrue、そうでない場合はfalse</returns>
    public virtual bool TryGetInputValueWithPort<T>(string toPortName, out T value)
    {
        if (TryGetInputValuesWithPort<T>(toPortName, out List<T> values))
        {
            value = values[0];
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// 指定したポートオブジェクトに関連付けられた入力値のリストを取得します。
    /// </summary>
    /// <typeparam name="T">取得する値の型</typeparam>
    /// <param name="toPort">取得対象のポート</param>
    /// <param name="value">取得された値のリスト</param>
    /// <returns>値が見つかった場合はtrue、そうでない場合はfalse</returns>
    public virtual bool TryGetInputValueWithPort<T>(Port toPort, out List<T> value)
    {
        return TryGetInputValuesWithPort<T>(toPort.portName, out value);
    }

    /// <summary>
    /// 指定した型と一致するすべての入力値を取得します。
    /// </summary>
    /// <typeparam name="T">取得する値の型</typeparam>
    /// <param name="value">取得された値のリスト</param>
    /// <returns>値が見つかった場合はtrue、そうでない場合はfalse</returns>
    public virtual bool TryGetInputValueWithType<T>(out List<T> value)
    {
        value = new List<T>();
        bool found = false;
        foreach (var data in inputValues)
        {
            if (data.value is T typedValue)
            {
                value.Add(typedValue);
                found = true;
            }
        }
        return found;
    }

    /// <summary>
    /// 出力ポートに接続されている先のポートの型を解析し、適切な接続型を返します。
    /// </summary>
    /// <returns>接続先のポートに基づいた型</returns>
    protected Type GetConnectionType()
    {
        Type connectionType = typeof(float);
        HashSet<Node> visitedNodes = new HashSet<Node>();
        foreach (var fromPort in outputPorts)
        {
            foreach (var toPort in fromPort.outputConections)
            {
                if (visitedNodes.Contains(toPort.node))
                {
                    continue;
                }
                visitedNodes.Add(toPort.node);
                foreach (var inputPort in toPort.node.inputPorts)
                {
                    if (inputPort.portName != toPort.portName)
                    {
                        continue;
                    }
                    if (inputPort.portType == typeof(Vector2))
                    {
                        return typeof(Vector2);
                    }
                    if (inputPort.portType == typeof(float))
                    {
                        connectionType = typeof(float);
                    }
                    else if (inputPort.portType == typeof(bool))
                    {
                        connectionType = typeof(bool);
                    }
                    connectionType = inputPort.portType;
                    break;
                }
            }
        }
        return connectionType;
    }
}
