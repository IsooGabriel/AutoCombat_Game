using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 外部ディレクトリからノード定義を読み込み、エディタUIを構築するクラス
/// </summary>
public class CustomNodeLoader : MonoBehaviour
{
    [SerializeField] private string customNodesPath = "CustomNodes";
    [SerializeField] private GameObject buttonPrefab; // AddNodeButtonのプレハブ
    [SerializeField] private Transform buttonParent; // ボタンを並べる親要素

    private void Start()
    {
        LoadAllCustomNodes();
    }

    public void LoadAllCustomNodes()
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, customNodesPath);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        CustomNodeRegistry.Clear();

        // JSONファイルの読み込み
        string[] files = Directory.GetFiles(fullPath, "*.json");
        foreach (var file in files)
        {
            string json = File.ReadAllText(file);
            NodeDefinition def = JsonUtility.FromJson<NodeDefinition>(json);
            if (def != null && !string.IsNullOrEmpty(def.typeName))
            {
                CustomNodeRegistry.Register(def);
                CreateAddNodeButton(def);
            }
        }
    }

    private void CreateAddNodeButton(NodeDefinition def)
    {
        if (buttonPrefab == null || buttonParent == null) return;

        GameObject btnObj = Instantiate(buttonPrefab, buttonParent);
        btnObj.name = $"AddNode_{def.typeName}";

        // テキストの更新
        var text = btnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null) text.text = def.displayName;

        // AddNodeButtonFuctoryがアタッチされている場合は無効化または調整
        var factory = btnObj.GetComponent<AddNodeButtonFuctory>();
        if (factory != null) factory.enabled = false;

        // クリックイベントの設定
        var button = btnObj.GetComponentInChildren<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            GraphEditorManager.Instance.AddCustomNode(def.typeName);
        });

        // カテゴリに基づいた色の設定（既存のロジックを流用可能）
        // ここでは簡易的にアウトラインの色を変える例
        var outline = btnObj.transform.Find("Outline")?.GetComponent<Image>();
        if (outline != null)
        {
            Color color = def.category switch
            {
                "ACTION" => new Color(0.9f, 0.1f, 0.2f),
                "MATH" => new Color(0.1f, 0.4f, 0.9f),
                "GETSET" => new Color(0.3f, 0.9f, 0.7f),
                _ => Color.white
            };
            outline.color = color;
        }
    }
}
