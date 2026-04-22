using UnityEngine;
using System.Linq;
using TMPro;
/// <summary>
/// ポートの数に合わせて、実行時にポートUIを生成・配置するNodeUIの拡張
/// </summary>
public class DynamicNodeUI : NodeUI
{
    [Header("Dynamic Port Settings")]
    [SerializeField] private GameObject inputPortPrefab; // 入力ポート用
    [SerializeField] private GameObject outputPortPrefab; // 出力ポート用
    [SerializeField] private Transform inputPortParent; 
    [SerializeField] private Transform outputPortParent; 

    public override void Awake()
    {
        if (node != null && string.IsNullOrEmpty(node.id))
        {
            node.id = System.Guid.NewGuid().ToString();
        }
    }

    public void RefreshDynamicPorts()
    {
        if (node == null) return;

        GenericNode genericNode = node as GenericNode;

        // タイトルの更新
        if (titleText == null)
        {
            // "Title" という名前の子オブジェクトを探すか、最初に見つかったTMPを使用する
            var titleObj = transform.Find("Title");
            if (titleObj != null) titleText = titleObj.GetComponent<TextMeshProUGUI>();
            
            if (titleText == null)
            {
                // ポート用コンテナ以外にあるテキストを探す
                titleText = GetComponentsInChildren<TextMeshProUGUI>()
                    .FirstOrDefault(t => t.transform.parent != inputPortParent && t.transform.parent != outputPortParent);
            }
        }

        if (titleText != null)
        {
            if (genericNode != null && genericNode.definition != null)
            {
                titleText.text = genericNode.definition.displayName;
            }
            else
            {
                titleText.text = node.nodeType.ToString();
                if (genericNode != null && genericNode.definition == null)
                {
                    Debug.LogWarning($"GenericNode definition missing for: {genericNode.customTypeName}");
                }
            }
        }

        // 既存をクリア
        foreach (Transform child in inputPortParent) Destroy(child.gameObject);
        foreach (Transform child in outputPortParent) Destroy(child.gameObject);

        // 入力生成
        inputPorts = node.inputPorts.Select(p => CreatePortUI(p, inputPortParent, inputPortPrefab)).ToArray();
        // 出力生成
        outputPorts = node.outputPorts.Select(p => CreatePortUI(p, outputPortParent, outputPortPrefab)).ToArray();

        SetOwnerInPorts(inputPorts);
        SetOwnerInPorts(outputPorts);

        // サイズの適用
        if (genericNode != null && genericNode.definition != null)
        {
            ApplySize(genericNode.definition);
        }
    }

    private void ApplySize(NodeDefinition def)
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (rect == null) return;

        float targetWidth = def.width > 0 ? def.width : rect.sizeDelta.x;
        float targetHeight = def.height;

        if (targetHeight <= 0)
        {
            // ポート数に基づいて高さを自動計算 (ポートごとに 20)
            int maxPorts = Mathf.Max(node.inputPorts.Length, node.outputPorts.Length);
            targetHeight = 10-3+(maxPorts * 23f);
        }

        rect.sizeDelta = new Vector2(targetWidth, targetHeight);

        // NodeMoveSystemの調整
        if (TryGetComponent<NodeMoveSystem>(out var moveSystem))
        {
            // ポートエリアの最上部を基準にしつつ、15px上にずらしてヘッダーを掴みやすくする
            moveSystem.pivotOffset = new Vector2(0f, -(targetHeight / 2f)- 15f);
        }
    }

    private PortUI CreatePortUI(Port port, Transform parent, GameObject prefab)
    {
        if (prefab == null) return null;
        GameObject go = Instantiate(prefab, parent);
        PortUI ui = go.GetComponent<PortUI>();
        ui.port = port;
        ui.owner = this;
        ui.portTypeHue = GetHueFromType(port.portType, port.isExecutionPort);
        return ui;
    }

    private PortUI.PortTypeHue GetHueFromType(System.Type type, bool isExec)
    {
        if (isExec) return PortUI.PortTypeHue.EXECUTION;
        if (type == typeof(float) || type == typeof(int)) return PortUI.PortTypeHue.DECIMAL;
        if (type == typeof(bool)) return PortUI.PortTypeHue.BOOLEAN;
        if (type == typeof(Vector2)) return PortUI.PortTypeHue.VECTOR;
        return PortUI.PortTypeHue.OTHER;
    }
}
