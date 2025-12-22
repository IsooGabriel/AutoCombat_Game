using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GraphRunner : MonoBehaviour
{
    public GameObject winResult;
    public GameObject loseResult;

    public Character player;
    public Character enemy;
    public TextAsset playerGraphJson;
    public TextAsset enemyGraphJson;

    private GraphExecutor _playerExecutor;
    private GraphExecutor _enemyExecutor;
    private float _tickTimer;
    private const float TickInterval = 1f / 30f; // 30tick/秒

    private bool isRunning = false;

    private readonly string graphPath = "GraphData";

    private async void InstanceExecutor()
    {
        string path = await OpenFileDialog();
        string json = File.ReadAllText(path);
        string enemypath = $"{(Application.persistentDataPath.Replace("/", "\\"))}\\{EnemyGraphLoader.graphPath}";
        string enemyjson = File.ReadAllText(enemypath);

        var playerGraph = JsonUtility.FromJson<GraphData>(json);
        var enemyGraph = JsonUtility.FromJson<GraphData>(enemyjson);
        //var playerGraph = JsonUtility.FromJson<GraphData>(playerGraphJson.text);
        //var enemyGraph = JsonUtility.FromJson<GraphData>(enemyGraphJson.text);
        if (playerGraph == null || enemyGraph == null || player == null || enemy == null)
        {
            Debug.LogError("なんかたんない");
            return;
        }
        _playerExecutor = new GraphExecutor(playerGraph, player, enemy);
        _enemyExecutor = new GraphExecutor(enemyGraph, enemy, player);
    }

    private void OnPlayerWin()
    { 
        Time.timeScale = 0f;
        winResult.SetActive(true);
    }
    private void OnEnemyWin()
    {
        Time.timeScale = 0f;
        loseResult.SetActive(true);
    }

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

    private Task<string> OpenFileDialog()
    {
        string prevDir = Environment.CurrentDirectory;
        try
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

            if (!Directory.Exists(ofn.lpstrInitialDir))
            {
                Directory.CreateDirectory(ofn.lpstrInitialDir);
            }
            if (GetOpenFileName(ref ofn))
            {
                return Task.FromResult<string>(ofn.lpstrFile);
            }
            return null;
        }
        finally
        {
            Environment.CurrentDirectory = prevDir;
        }
    }

    void Start()
    { 
        Time.timeScale = 1f;
        Application.targetFrameRate = 30;

        InstanceExecutor();
        isRunning = true;
    }

    void Update()
    {

        if (enemy == null)
        {
            OnPlayerWin();
            isRunning = false;
        }
        else if (player == null)
        {
            OnEnemyWin();
            isRunning = false;
        }

        if (!isRunning)
        {
            return;
        }

        _tickTimer += Time.deltaTime;

        if (_tickTimer >= TickInterval)
        {
            _tickTimer -= TickInterval;
            _playerExecutor.ExecuteTick();
            _enemyExecutor.ExecuteTick();
        }
    }
}