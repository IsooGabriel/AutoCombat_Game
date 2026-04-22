using UnityEngine;
using TMPro;

public class NodeUI : MonoBehaviour
{
    public Node node = new _DebugNode(); // このUIに関連付けられたロジック用ノード
    [SerializeField]
    public PortUI[] inputPorts; // 入力ポートのUIコンポーネント配列
    [SerializeField]
    public PortUI[] outputPorts; // 出力ポートのUIコンポーネント配列
    [SerializeField]
    public TextMeshProUGUI titleText; // ノードの名前を表示するテキスト

    /// <summary>
    /// 指定されたポートUI配列の各要素に対して、所有者となるNodeUIとNodeを設定します。
    /// </summary>
    /// <param name="ports">所有者を設定するポートUIの配列</param>
    public void SetOwnerInPorts(PortUI[] ports)
    {
        foreach (var portUI in ports)
        {
            if (portUI == null || portUI.port == null)
            {
                continue;
            }
            portUI.owner = this;
            portUI.port.owner = node;
        }
    }

    /// <summary>
    /// コンポーネントの起動時にノードのIDを生成し、各ポートの所有者を初期化します。
    /// </summary>
    public virtual void Awake()
    {
        node.id = System.Guid.NewGuid().ToString();
        if (inputPorts != null && inputPorts.Length > 0)
        {
            SetOwnerInPorts(inputPorts);
        }
        if (outputPorts != null && outputPorts.Length > 0)
        {
            SetOwnerInPorts(outputPorts);
        }
    }
}
