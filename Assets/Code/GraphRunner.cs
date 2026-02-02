using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;


public class GraphRunner : MonoBehaviour
{
    public GameObject winResult;
    public GameObject loseResult;

    [SerializeField]
    private WeaponDB weaponDB;
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

    readonly private float submitTimeScale = 0.05f;
    readonly private float minTimeScale = 0.07f;


    private async void InstanceExecutor()
    {
        string path = await OpenFileDialog();
        string json = File.ReadAllText(path);
        string enemypath = $"{(Application.persistentDataPath.Replace("/", "\\"))}\\{EnemyGraphLoader.graphPath}";
        string enemyjson = File.ReadAllText(enemypath);

        var playerGraph = JsonUtility.FromJson<GraphData>(json);
        GraphEditorManager.Instance ??= new GraphEditorManager();
        GraphEditorManager.Instance.AjustAdditionalStatus(playerGraph.aditionalStatus);
        var enemyGraph = JsonUtility.FromJson<GraphData>(enemyjson);
        //var playerGraph = JsonUtility.FromJson<GraphData>(playerGraphJson.text);
        //var enemyGraph = JsonUtility.FromJson<GraphData>(enemyGraphJson.text);
        if (playerGraph == null || enemyGraph == null || player == null || enemy == null)
        {
            Debug.LogError("なんかたんない");
            return;
        }

        SetWeaponCharacter(player, playerGraph);
        SetWeaponCharacter(enemy, enemyGraph);

        _playerExecutor = new GraphExecutor(playerGraph, player, enemy);
        _enemyExecutor = new GraphExecutor(enemyGraph, enemy, player);
        player.Start();
        enemy.Start();
    }

    private void OnPlayerWin()
    {
        winResult.SetActive(true);
    }
    private void OnEnemyWin()
    {
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

    private void SetWeaponCharacter(Character chara, GraphData graph)
    {
        Weapon.Weapon weapon;
        if (graph.weapon != GraphData.noWeapon || graph.weapon > -1)
        {
            GameObject weponObj = Instantiate
                (
                    weaponDB.weaponDatas[graph.weapon].prefab,
                    chara.transform
                );
            weponObj.transform.localPosition = Vector3.zero;
            weapon = weponObj.GetComponent<Weapon.Weapon>();
        }
        else
        {
            weapon = null;
        }
        chara.weapon = weapon;
        weapon.user = chara;
    }

    void Start()
    {
        Time.timeScale = 1f;
        Application.targetFrameRate = 30;

        InstanceExecutor();
        isRunning = true;
    }
    private void FixedUpdate()
    {
        if (enemy == null || player == null)
        {
            if (Time.timeScale <= minTimeScale)
            {
                Time.timeScale = 0;
                return;
            }
            Time.timeScale -= Time.timeScale * Time.deltaTime + submitTimeScale;
            Debug.Log(Time.timeScale + "============");
        }
    }
    void Update()
    {

        if (enemy == null)
        {
            OnPlayerWin();
            isRunning = false;
            return;
        }
        else if (player == null)
        {
            OnEnemyWin();
            isRunning = false;
            return;
        }

        Debug.Log(enemy.currentHP + "============");

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