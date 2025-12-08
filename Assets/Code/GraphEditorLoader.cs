using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class GraphEditorLoader : MonoBehaviour
{
    [SerializeField]
    private GraphEditorManager manager;
    private readonly string graphPath= "GraphData";
    private string path;
    private GraphData graphData;

    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName(ref OpenFileName ofn);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct OpenFileName
    {
        public int lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;
        public string lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public string lpstrFile;
        public int nMaxFile;
        public string lpstrFileTitle;
        public int nMaxFileTitle;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public IntPtr lCustData;
        public IntPtr lpfnHook;
        public string lpTemplateName;
        public IntPtr pvReserved;
        public int dwReserved;
        public int FlagsEx;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string path = OpenFileDialog();
            if (!string.IsNullOrEmpty(path))
            {
                Debug.Log("選択されたファイル: " + path);
            }
        }
    }

    string OpenFileDialog()
    {
        OpenFileName ofn = new OpenFileName();
        ofn.lStructSize = Marshal.SizeOf(ofn);
        ofn.lpstrFilter = "グラフデータ\0*.json;*.acjson\0";
        ofn.lpstrFile = new string(new char[256]);
        ofn.nMaxFile = ofn.lpstrFile.Length;
        ofn.lpstrFileTitle = new string(new char[64]);
        ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
        ofn.lpstrTitle = "ファイルを選択";
        ofn.lpstrInitialDir = $"{Application.persistentDataPath.Replace("/", "\\")}\\{graphPath}\\";

        if (GetOpenFileName(ref ofn))
        {
            return ofn.lpstrFile;
        }
        return null;
    }

    public void SelectFile()
    {
       this.path = OpenFileDialog().Replace("/", "\\");
        if (!string.IsNullOrEmpty(path))
        {
            LoadJson(path);
            LoadEditor();
        }
    }
    public void LoadJson(string path)
    {
        this.path = path;
        string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
        graphData = JsonUtility.FromJson<GraphData>(json);
    }

    public void LoadEditor()
    {
        GameObject prefab = null;
        NodeUI nodeUI = null;
        foreach (var nodeData in graphData.nodes)
        {
            prefab = manager.nodePrefabs.First(p => p.type == nodeData.type).prefab;
            if (prefab == null)
            {
                continue;
            }
            prefab = Instantiate(prefab, new Vector3(nodeData.position.x, nodeData.position.y, 0), Quaternion.identity);
            nodeUI = prefab.GetComponent<NodeUI>();
            if (prefab == null)
            {
                continue;
            }
            if (nodeUI == null)
            {
                Destroy(prefab);
                continue;
            }


            if(nodeUI is SetValueNodeUI setValueNodeUI)
            {
                if(nodeData.inputValues != null || nodeData.inputValues.Count != 0)
                {
                    foreach (var inputValue in nodeData.inputValues)
                    {
                        if (inputValue.toPortName != SetValueNodeUI.settingKey)
                        {
                            continue;
                        }
                        setValueNodeUI.SetData((float)inputValue.value);
                    }
                }
                setValueNodeUI.SetData();
            }
            else if(nodeUI is IfNodeUI ifNodeUI)
            {
                if(nodeData.inputValues != null || nodeData.inputValues.Count != 0)
                {
                    foreach (var inputValue in nodeData.inputValues)
                    {
                        if (inputValue.toPortName != IfNodeUI.settingKey)
                        {
                            continue;
                        }
                        ifNodeUI.SetSetting((IfSettings)(int)inputValue.value);
                    }
                }
                ifNodeUI.SetSetting();
            }
            else if(nodeUI is GetPositionNodeUI getPositionNodeUI)
            {
                if(nodeData.inputValues != null || nodeData.inputValues.Count != 0)
                {
                    foreach (var inputValue in nodeData.inputValues)
                    {
                        if (inputValue.toPortName != GetPositionNode.positionTypeDataName)
                        {
                            continue;
                        }
                        getPositionNodeUI.SetSetting((GetPositionSettings)(int)inputValue.value);
                    }
                }
                getPositionNodeUI.SetSetting();
            }
                prefab.transform.parent = manager.nodesParent.transform;
            nodeUI.node.SetData(nodeData);
            manager.nodeUIs.Add(nodeUI);
            nodeUI.node.Initialize();
        }
    }
}
