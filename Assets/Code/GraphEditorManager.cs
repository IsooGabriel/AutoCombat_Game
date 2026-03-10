using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GraphEditorManager : MonoBehaviour
{
    public Camera graphCamera; // グラフエディタ用のカメラ

    [NonSerialized]
    public readonly string editorLayer = "GraphEditor"; // エディタ用オブジェクトのレイヤー名

    static public GraphEditorManager Instance = null; // GraphEditorManagerのシングルトンインスタンス
    [NonSerialized]
    public bool isSelected = false; // 現在ノードやポートが選択されているか
    [NonSerialized]
    public PortUI selectedPort = null; // 現在選択されているポートUI
    private float lineWidth = 1f; // 接続線の幅
    [NonSerialized]
    public List<NodeUI> nodeUIs = new(); // 配置されているNodeUIのリスト
    public GameObject nodesParent; // 生成されたノードの親オブジェクト
    public NodePrefab[] nodePrefabs; // ノードの種類とプレハブの対応配列
    public GraphData graphData = new(); // 編集中のグラフデータ

    public readonly float portUISaturation = 0.7f; // ポートUIの彩度
    public readonly float portUIValue = 0.8f; // ポートUIの明度
    public readonly float portUISerectSaturation = 0.25f; // ポート選択時の彩度
    public readonly float portUISerectValue = 0.9f; // ポート選択時の明度

    public TMP_InputField additionalHP; // HP追加値入力用UI
    public TMP_InputField additionalAttack; // 攻撃力追加値入力用UI
    public TMP_InputField additionalAttackCT; // 攻撃クールタイム追加値入力用UI
    public TMP_InputField additionalCriticalChance; // クリティカル率追加値入力用UI
    public TMP_InputField additionalCriticalDamage; // クリティカルダメージ追加値入力用UI

    public PlayerInput playerInput; // 入力システム
    public EventSystem eventSystem; // Unityのイベントシステム
    public GraphicRaycaster raycaster; // UIレイキャスト用

    public const string defaultPath = "GraphData"; // グラフデータのデフォルト保存先
    public const string enemyPath = "EnemyGraph"; // 敵グラフデータの保存先
    public const string playerDataFileName = "player.acjson"; // プレイヤーデータのデフォルトファイル名
    public const string enemyDataFileName = "enemy.json"; // 敵データのデフォルトファイル名
    public const string nameSpacer = ", "; // グラフ名や製作者名の区切り文字
    public readonly int[] maxNodesForPoints = { 987, 610, 377, 233, 144, 89, 55, 34, 21, 13 }; // ノード数に応じたステータス加算ポイントの閾値
    private int aditionableStatusCount = 0; // 現在加算されているステータスの合計値
    private GraphEditorLoader loader; // グラフ読み込み用ローダー
    [SerializeField]
    private GameObject loadObject; // ロード中表示用UIオブジェクト
    [SerializeField]
    private Slider loadSlider; // ロード進捗表示用スライダー
    public Action onLoardGraph; // グラフ読み込み完了時のコールバック
    public Action onChangeNodeCount; // ノード数が変更された時のコールバック

    #region 関数

    /// <summary>
    /// 指定された種類のノードを新しくグラフに追加します。
    /// </summary>
    /// <param name="type">追加するノードの種類</param>
    public void AddNode(NodeType type)
    {
        Node node = NodeFactory.Create(type);
        node.EditorInitialize();
        node.position = Vector2.zero;

        NodeUI nodeUI = Instantiate(
            GraphEditorManager.Instance.nodePrefabs.FirstOrDefault(p => p.type == type).prefab,
            GraphEditorManager.Instance.nodesParent.transform
        ).GetComponent<NodeUI>();
        nodeUI.node = node;
        nodeUI.transform.position = node.position;

        SetPortUIsPort(nodeUI.inputPorts, node.inputPorts);
        SetPortUIsPort(nodeUI.outputPorts, node.outputPorts);

        Instance.nodeUIs.Add(nodeUI);
        Instance.AjustAdditionalStatus(Instance.graphData.aditionalStatus);
        Instance.UpdateAdditionalStatus();

        onChangeNodeCount?.Invoke();
    }

    /// <summary>
    /// PortUI配列に対してNodeのPort情報を紐付け、不要なUIを削除します。
    /// </summary>
    /// <param name="portUIs">対象となるPortUI配列</param>
    /// <param name="ports">紐付けるPort配列</param>
    public void SetPortUIsPort(PortUI[] portUIs, Port[] ports)
    {
        for (int i = 0; i < portUIs.Length; ++i)
        {
            if (ports == null || ports.Length <= i || ports[i] == null)
            {
                if (portUIs[i] != null)
                {
                    Destroy(portUIs[i].gameObject);
                }
                continue;
            }
            portUIs[i].port = ports[i];
        }
    }

    /// <summary>
    /// 文字列で指定された種類のノードを新しくグラフに追加します。
    /// </summary>
    /// <param name="typeName">追加するノードの種類名</param>
    public void AddNode(string typeName)
    {
        NodeType type;
        Enum.TryParse(typeName, out type);
        AddNode(type);
    }


    /// <summary>
    /// カーソル位置にあるノードまたは接続を削除します（Inputアクション用）。
    /// </summary>
    /// <param name="context">入力アクションのコンテキスト</param>
    public void DeleteNode(InputAction.CallbackContext context)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        if (results.Count == 0)
        {
            return;
        }

        GameObject topUI = results[0].gameObject;
        PortUI portUI = topUI.GetComponentInParent<PortUI>();
        if (portUI != null)
        {
            DisconectPort(portUI);
            return;
        }

        NodeUI nodeUI = topUI.GetComponentInParent<NodeUI>();
        if (nodeUI.node is StartNode)
        {
            return;
        }

        DeleteNode(nodeUI);
    }

    /// <summary>
    /// 指定されたNodeUIおよびそれに関連する接続をグラフから削除します。
    /// </summary>
    /// <param name="nodeUI">削除するNodeUIのインスタンス</param>
    public void DeleteNode(NodeUI nodeUI)
    {
        Node deleteNode = nodeUI.node;

        Instance.nodeUIs.Remove(nodeUI);

        foreach (var nodeDeta in Instance.nodeUIs)
        {
            foreach (var port in nodeDeta.node.outputPorts)
            {
                port.outputConections.RemoveAll(c => c.node.id == deleteNode.id);
            }
        }

        Destroy(nodeUI.gameObject);
        onChangeNodeCount?.Invoke();
    }

    /// <summary>
    /// 現在のグラフをリセットし、すべてのノードを削除します。
    /// </summary>
    public void ResetGraph()
    {
        graphData = null;
        foreach (var nodeUI in nodeUIs)
        {
            Destroy(nodeUI.gameObject);
        }
        nodeUIs.Clear();
        aditionableStatusCount = 0;
        onChangeNodeCount?.Invoke();
    }

    #region 追加ステータス

    /// <summary>
    /// UI上の追加ステータス表示を現在のデータに合わせて更新します。
    /// </summary>
    public void UpdateAdditionalStatus()
    {
        additionalHP.text = $"{Instance.graphData.aditionalStatus.hp}";
        additionalAttack.text = $"{Instance.graphData.aditionalStatus.attack}";
        additionalAttackCT.text = $"{Instance.graphData.aditionalStatus.attackCooltime}";
        additionalCriticalChance.text = $"{Instance.graphData.aditionalStatus.criticalChance}";
        additionalCriticalDamage.text = $"{Instance.graphData.aditionalStatus.criticalDamage}";
    }

    /// <summary>
    /// 追加ステータスの値を制限範囲内で設定します。
    /// </summary>
    /// <param name="value">設定しようとする値</param>
    /// <param name="status">設定対象となるステータス変数への参照</param>
    public void SetAdditionalStatus(int value, ref int status)
    {
        if (value < 0)
        {
            value = 0;
        }
        int old = status;
        if (aditionableStatusCount - old + value > Instance.CheckMaxAdditionableStatus())
        {
            value = Instance.CheckMaxAdditionableStatus() - (aditionableStatusCount - old);
        }
        aditionableStatusCount += value - old;
        status = value;
        Instance.UpdateAdditionalStatus();
    }

    /// <summary>
    /// 追加HPを設定します。
    /// </summary>
    /// <param name="value">設定する値</param>
    public void SetHP(int value)
    {
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.hp);
    }
    /// <summary>
    /// 文字列から追加HPを設定します。
    /// </summary>
    /// <param name="value">設定する値の文字列</param>
    public void SetHP(string value)
    {
        Instance.SetHP(int.Parse(value));
    }
    /// <summary>
    /// 現在の追加HPに値を加算します。
    /// </summary>
    /// <param name="value">加算する値</param>
    public void AddHP(int value)
    {
        value += Instance.graphData.aditionalStatus.hp;
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.hp);
    }


    /// <summary>
    /// 追加攻撃力を設定します。
    /// </summary>
    /// <param name="value">設定する値</param>
    public void SetAttack(int value)
    {
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.attack);
    }
    /// <summary>
    /// 文字列から追加攻撃力を設定します。
    /// </summary>
    /// <param name="value">設定する値の文字列</param>
    public void SetAttack(string value)
    {
        Instance.SetAttack(int.Parse(value));
    }
    /// <summary>
    /// 現在の追加攻撃力に値を加算します。
    /// </summary>
    /// <param name="value">加算する値</param>
    public void AddAttack(int value)
    {
        value += Instance.graphData.aditionalStatus.attack;
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.attack);
    }


    /// <summary>
    /// 追加攻撃クールタイムを設定します。
    /// </summary>
    /// <param name="value">設定する値</param>
    public void SetAttackCT(int value)
    {
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.attackCooltime);
    }
    /// <summary>
    /// 文字列から追加攻撃クールタイムを設定します。
    /// </summary>
    /// <param name="value">設定する値の文字列</param>
    public void SetAttackCT(string value)
    {
        Instance.SetAttackCT(int.Parse(value));
    }

    /// <summary>
    /// 現在の追加攻撃クールタイムに値を加算します。
    /// </summary>
    /// <param name="value">加算する値</param>
    public void AddAttackCT(int value)
    {
        value += Instance.graphData.aditionalStatus.attackCooltime;
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.attackCooltime);
    }


    /// <summary>
    /// 追加クリティカル率を設定します。
    /// </summary>
    /// <param name="value">設定する値</param>
    public void SetCriticalChance(int value)
    {
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.criticalChance);
    }
    /// <summary>
    /// 文字列から追加クリティカル率を設定します。
    /// </summary>
    /// <param name="value">設定する値の文字列</param>
    public void SetCriticalChance(string value)
    {
        Instance.SetCriticalChance(int.Parse(value));
    }
    /// <summary>
    /// 現在の追加クリティカル率に値を加算します。
    /// </summary>
    /// <param name="value">加算する値</param>
    public void AddCriticalChance(int value)
    {
        value += Instance.graphData.aditionalStatus.criticalChance;
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.criticalChance);
    }


    /// <summary>
    /// 追加クリティカルダメージを設定します。
    /// </summary>
    /// <param name="value">設定する値</param>
    public void SetCriticalDamage(int value)
    {
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.criticalDamage);
    }
    /// <summary>
    /// 文字列から追加クリティカルダメージを設定します。
    /// </summary>
    /// <param name="value">設定する値の文字列</param>
    public void SetCriticalDamage(string value)
    {
        Instance.SetCriticalDamage(int.Parse(value));
    }
    /// <summary>
    /// 現在の追加クリティカルダメージに値を加算します。
    /// </summary>
    /// <param name="value">加算する値</param>
    public void AddCriticalDamage(int value)
    {
        value += Instance.graphData.aditionalStatus.criticalDamage;
        Instance.SetAdditionalStatus(value, ref Instance.graphData.aditionalStatus.criticalDamage);
    }

    /// <summary>
    /// ステータスを追加可能かどうかをチェックします。
    /// </summary>
    /// <param name="addition">追加しようとしている量</param>
    /// <returns>追加可能な場合はtrue</returns>
    public bool CheckAdditionableStatus(int addition)
    {
        return aditionableStatusCount + addition <= CheckMaxAdditionableStatus();
    }

    /// <summary>
    /// ノード数に基づいた追加可能なステータスポイントの最大値を返します。
    /// </summary>
    /// <param name="nodeCount">現在のノード数</param>
    /// <returns>追加可能なポイント数</returns>
    public int CheckMaxAdditionableStatus(int nodeCount)
    {
        for (int i = 0; maxNodesForPoints.Length > i; ++i)
        {
            if (nodeCount > maxNodesForPoints[i])
            {
                return i;
            }
        }
        return maxNodesForPoints.Length;
    }
    /// <summary>
    /// 現在のグラフのノード数に基づいた最大追加可能ステータスポイントを返します。
    /// </summary>
    /// <returns>追加可能なポイント数</returns>
    public int CheckMaxAdditionableStatus()
    {
        return CheckMaxAdditionableStatus(Instance.nodeUIs.Count);
    }

    /// <summary>
    /// ノード数による制限に合わせて追加ステータスを自動調整します。
    /// </summary>
    /// <param name="status">調整対象のステータスデータ</param>
    /// <returns>最終的なステータス合計値</returns>
    public int AjustAdditionalStatus(Status status)
    {
        int max = CheckMaxAdditionableStatus();
        int sum = status.hp + status.attack + status.attackCooltime + status.criticalChance + status.criticalDamage;
        if (sum <= max)
        {
            return sum;
        }

        for (int i = sum - max; i > 0; --i)
        {
            if (status.hp > 0)
            {
                status.hp -= 1;
                continue;
            }
            else if (status.attack > 0)
            {
                status.attack -= 1;
                continue;
            }
            else if (status.attackCooltime > 0)
            {
                status.attackCooltime -= 1;
                continue;
            }
            else if (status.criticalChance > 0)
            {
                status.criticalChance -= 1;
                continue;
            }
            else if (status.criticalDamage > 0)
            {
                status.criticalDamage -= 1;
                continue;
            }
        }
        return sum - max;
    }

    /// <summary>
    /// グラフで使用するスキンのIDを変更します。
    /// </summary>
    /// <param name="skinID">設定するスキンID</param>
    public void ChangeSkin(int skinID)
    {
        Instance.graphData.skin = skinID;
    }

    #endregion


    /// <summary>
    /// 2つのポートを接続します。
    /// </summary>
    /// <param name="from">接続元ポートUI</param>
    /// <param name="to">接続先ポートUI</param>
    static public void ConectPorts(PortUI from, PortUI to)
    {
        if (from.port.isToPort && !to.port.isToPort)
        {
            (from, to) = (to, from);
        }
        if (!GraphEditorManager.Instance.CheckConectable(from, to))
        {
            Debug.Log("接続不可能");
            return;
        }
        if (from.port.outputConections == null)
        {
            from.port.outputConections = new List<(Node, string)>();
        }

        from.port.outputConections.Add((to.port.owner, to.port.portName));
        if (from.port.owner is LinkedNode fromLinkedNode)
        {
            Array.Resize(ref fromLinkedNode.fromNodes, fromLinkedNode.fromNodes.Length + 1);
            fromLinkedNode.fromNodes[fromLinkedNode.fromNodes.Length - 1] = to.port.owner;
        }
        if (to.port.owner is LinkedNode linkedNode)
        {
            Array.Resize(ref linkedNode.toNodes, linkedNode.toNodes.Length + 1);
            linkedNode.toNodes[linkedNode.toNodes.Length - 1] = from.port.owner;
        }

        GraphEditorManager.Instance.SetLine(from, to);
        Debug.Log("接続完了");
    }

    /// <summary>
    /// 2つのポートが接続可能かどうかを判定します。
    /// </summary>
    /// <param name="from">接続元候補</param>
    /// <param name="to">接続先候補</param>
    /// <returns>接続可能な場合はtrue</returns>
    public bool CheckConectable(PortUI from, PortUI to)
    {
        if (from == null || to == null)
        {
            return false;
        }
        if (from.owner.node.id == to.owner.node.id)
        {
            return false;
        }
        if (from.port.isToPort || !to.port.isToPort)
        {
            return false;
        }
        if (from.port.isExecutionPort ^ to.port.isExecutionPort)
        {
            return false;
        }
        if (from.port.portType != to.port.portType || from.port.portType.GetType() != to.port.portType.GetType())
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 接続されたポート間にLineRendererを使用して可視化線を生成します。
    /// </summary>
    /// <param name="from">接続元のポートUI</param>
    /// <param name="to">接続先のポートUI</param>
    private void SetLine(PortUI from, PortUI to)
    {
        GameObject conectionObj = new GameObject();
        ConectionUI conection = conectionObj.AddComponent<ConectionUI>();
        conection.fromPort = from;
        conection.toPort = to;
        conection.name = $"from_{from.port.portName}_to_{to.port.portName}";
        conection.name = $"from{from.name}OF{from.owner.node.id}to{to.name}OF{to.owner.node.id}";
        conection.transform.parent = from.transform;
        if (conection.lineRenderer == null)
        {
            conection.lineRenderer = conectionObj.AddComponent<LineRenderer>();
        }
        LineRenderer line = conection.lineRenderer;
        line.positionCount = 2;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = GraphEditorManager.Instance.lineWidth;
        line.endWidth = GraphEditorManager.Instance.lineWidth;
        line.startColor = Color.HSVToRGB(((float)from.portTypeHue) / 360, portUISaturation, portUIValue);
        line.endColor = Color.HSVToRGB(((float)to.portTypeHue) / 360, portUISaturation, portUIValue);
        line.SetPosition(0, from.portPosition.position);
        line.SetPosition(1, to.portPosition.position);
        line.gameObject.layer = LayerMask.NameToLayer(Instance.editorLayer);
        from.outputLines.Add(line);
        from.outputConectionsUI.Add(conection);
    }

    /// <summary>
    /// 指定されたポートの接続を解除します。
    /// </summary>
    /// <param name="target">切断対象のポートUI</param>
    public void DisconectPort(PortUI target)
    {
        if (!target.port.isToPort)
        {
            foreach (var line in target.outputLines)
            {
                Destroy(line.gameObject);
            }
            target.DisconectFromThis();
        }
        else
        {
            foreach (var node in Instance.nodeUIs)
            {
                foreach (var portUI in node.outputPorts)
                {
                    portUI.DisconectWithPortUI(target);
                }
            }
        }
    }


    /// <summary>
    /// グラフデータを非同期で保存します。
    /// </summary>
    /// <param name="path">保存先のファイルパス</param>
    /// <param name="graphName">グラフの名前</param>
    /// <param name="author">製作者名</param>
    public async Task SaveGraph(string path, string graphName, string author)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        loadObject.SetActive(true);

        HashSet<string> usedNodeIds = new HashSet<string>() { };
        Instance.graphData.nodes.Clear();

        NodeUI nodeUI;
        for (int i = 0; i < GraphEditorManager.Instance.nodeUIs.Count; ++i)
        {
            loadSlider.value += ((float)1 / Instance.nodeUIs.Count) * 0.8f * loadSlider.maxValue;

            nodeUI = Instance.nodeUIs[i];
            if (usedNodeIds.Contains(nodeUI.node.id))
            {
                continue;
            }
            if (stopwatch.ElapsedMilliseconds > 3)
            {
                stopwatch.Restart();
                await Task.Yield();
            }
            nodeUI.node.position = nodeUI.transform.position;
            NodeData nodeData = GenerateNodeData(nodeUI);
            Instance.graphData.nodes.Add(nodeData);
            usedNodeIds.Add(nodeUI.node.id);

            if (nodeUI.node is LinkedNode linkedNode)
            {
                List<string> inputIDs = new List<string>();
                List<string> outputIDs = new List<string>();
                foreach (var inputNode in linkedNode.toNodes)
                {
                    inputIDs.Add(inputNode.id);
                }
                foreach (var outputNode in linkedNode.fromNodes)
                {
                    outputIDs.Add(outputNode.id);
                }
                LinkedNodeData linkedNodeData = new LinkedNodeData(linkedNode.id, inputIDs, outputIDs);
                Instance.graphData.linkedNodes.Add(linkedNodeData);
            }
        }
        Instance.AjustAdditionalStatus(Instance.graphData.aditionalStatus);
        Instance.graphData.createdDate = DateTime.Now;
        if (Instance.graphData.graphName.Contains(nameSpacer))
        {
            Instance.graphData.graphName = graphName + nameSpacer + Instance.graphData.graphName;
        }
        else
        {
            Instance.graphData.graphName += graphName;
        }
        if (Instance.graphData.author.Contains(nameSpacer))
        {
            Instance.graphData.author = author + nameSpacer + Instance.graphData.author;
        }
        else
        {
            Instance.graphData.author += author;
        }

        Instance.graphData.version = GraphData.currentVersion;
        string json = JsonUtility.ToJson(Instance.graphData, true);
        EnsureDirectoryExists(path);

        loadSlider.value = 0.9f * loadSlider.maxValue;


        File.WriteAllText(path, json, System.Text.Encoding.UTF8);

        loadObject.SetActive(false);
    }
    /// <summary>
    /// グラフ名を指定してグラフを保存します。
    /// </summary>
    /// <param name="graphName">グラフの名前</param>
    /// <param name="author">製作者名</param>
    public void SaveGraph(string graphName, string author)
    {
        string parent = Application.persistentDataPath.Replace("/", "\\");

        SaveGraph($"{parent}\\{defaultPath}\\{SanitizeFileName(graphName)}-{SanitizeFileName(author)}__{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.acjson", graphName, author);
    }
    /// <summary>
    /// 指定されたパスにデフォルト名でグラフを保存します。
    /// </summary>
    /// <param name="path">保存先のフルパス</param>
    public async Task SaveGraph(string path)
    {
        await SaveGraph(path, $"_DEFAULT_{System.Guid.NewGuid().ToString()}", $"_DEFAULT_{System.Guid.NewGuid().ToString()}");
    }

    /// <summary>
    /// デフォルトのプレイヤーデータ名でグラフを保存します。
    /// </summary>
    public async Task SaveGraph()
    {
        await SaveGraph("PlaeyreData.json");
    }
    /// <summary>
    /// デフォルトの敵データ名でグラフを保存します。
    /// </summary>
    public void SaveEnemyGraph()
    {
        SaveGraph(enemyDataFileName);
    }
    /// <summary>
    /// プレイヤーまたは敵のデフォルトパスにグラフを保存します。
    /// </summary>
    /// <param name="isPlayer">プレイヤーデータとして保存するかどうか</param>
    public void SaveGraphDefaultPath(bool isPlayer = true)
    {
        string parent = Application.persistentDataPath.Replace("/", "\\");
        if (isPlayer)
        {
            SaveGraph($"{parent}\\{defaultPath}\\{playerDataFileName}");
        }
        else
        {
            SaveGraph($"{parent}\\{enemyPath}\\{enemyDataFileName}");
        }
    }

    /// <summary>
    /// ファイル選択ダイアログを表示してグラフを保存します。
    /// </summary>
    public async void SaveWithDialog()
    {
        string path = await loader.OpenFileDialog();
        if (path == GraphEditorLoader.chanceledMessage)
        {
            return;
        }
        if (string.IsNullOrEmpty(path))
        {
            string parent = Application.persistentDataPath.Replace("/", "\\");
            path =
                $"{parent}\\{defaultPath}\\{playerDataFileName}";
        }
        if (!path.EndsWith(".acjson") && !path.EndsWith(".json") && !path.Contains("."))
        {
            path += ".acjson";
        }
        SaveGraph(path);
    }

    /// <summary>
    /// 文字列からファイル名として使用できない文字を除去・置換します。
    /// </summary>
    /// <param name="name">対象の文字列</param>
    /// <returns>サニタイズされた文字列</returns>
    public static string SanitizeFileName(string name)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
        {
            name = name.Replace(c, '_');
        }
        name = name.Replace('/', '_').Replace('\\', '_');
        return name;
    }
    /// <summary>
    /// 指定されたパスのディレクトリが存在しない場合に作成します。
    /// </summary>
    /// <param name="path">チェックするディレクトリパス</param>
    private void EnsureDirectoryExists(string path)
    {
        return;
        path = Path.GetDirectoryName(path);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// NodeUIの情報を元にシリアライズ可能なNodeDataを生成します。
    /// </summary>
    /// <param name="nodeUI">元となるNodeUI</param>
    /// <returns>生成されたNodeData</returns>
    public NodeData GenerateNodeData(NodeUI nodeUI)
    {
        Node node = nodeUI.node;
        NodeData nodeData = new NodeData(node.id, node.nodeType, nodeUI.transform.position, new() { });
        if (node.outputPorts == null || node.outputPorts.Length == 0)
        {
            return nodeData;
        }

        for (int i = 0; i < node.outputPorts.Length; ++i)
        {
            nodeData.outputConnections.Add(new PortConections(node.outputPorts[i].portName, new() { }));

            if (node.outputPorts[i].outputConections == null || node.outputPorts[i].outputConections.Count == 0)
            {
                continue;
            }

            for (int j = 0; j < node.outputPorts[i].outputConections.Count; ++j)
            {
                nodeData.outputConnections[i].toPortNodes.Add(
                    new PortOfNode(
                        node.outputPorts[i].outputConections[j].node.id,
                        node.outputPorts[i].outputConections[j].portName
                    )
                );
            }
        }
        for (int i = 0; i < node.inputValues.Count; ++i)
        {
            if (node.inputValues[i] == null)
            {
                continue;
            }
            if (node.inputValues[i].value.GetType() != typeof(float))
            {
                continue;
            }
            if (nodeData.inputValues == null)
            {
                nodeData.inputValues = new List<InputValue<float>>() { };
            }
            nodeData.inputValues.Add(new InputValue<float>(node.inputValues[i].toPortName, (float)node.inputValues[i].value, isUserset: true));
        }
        Debug.Log("ノードデータ生成完了");
        return nodeData;
    }

    public async void ReloadPreview()
    {
        string path = $"{Application.persistentDataPath.Replace("/", "\\")}\\{defaultPath}\\{playerDataFileName}";
        await SaveGraph(path);
        ResetGraph();
        graphData = loader.LoadJson(path);
        await loader.LoadEditor();
        PreviewRunner.Instance.Reload();
    }

    #endregion


    private void Awake()
    {
        Time.timeScale = 1f;
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        if (Instance == null)
        {
            Instance = this;
        }
        Instance.graphData = new GraphData()
        {
            nodes = new List<NodeData>(),
        };
        AddNode(NodeType.Start);
        UpdateAdditionalStatus();
        onLoardGraph += UpdateAdditionalStatus;
        onLoardGraph += () => onChangeNodeCount?.Invoke();
        if(loader == null)
        {
            loader = GetComponent<GraphEditorLoader>();
        }
        if (loader == null)
        {
            loader = new GraphEditorLoader();
        }
        SceneManager.LoadScene("Preview", LoadSceneMode.Additive);
    }

    private void OnEnable()
    {
        playerInput.actions["Delete"].performed += DeleteNode;
    }
}

[Serializable]
public class NodePrefab
{
    public NodeType type; // ノードの種類
    public GameObject prefab; // 対応するプレハブ
}


public interface IInteractable
{
    void Delete();
}
