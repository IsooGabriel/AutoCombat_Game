using System.IO;
using System.Threading.Tasks;
using UnityEngine;


public class PreviewRunner : MonoBehaviour
{
    public static PreviewRunner Instance { get; private set; }
    [SerializeField]
    private WeaponDB weaponDB;
    public Character player;
    private Character clonePlayer;
    public Character enemy;
    private Character cloneEnemy;

    [SerializeField]
    private GraphExecutor _playerExecutor;
    private float _tickTimer;
    private const float TickInterval = 1f / 30f; // 30tick/•b

    private bool isRunning = false;

    private async Task InstanceExecutor()
    {
        string path = $"{(Application.persistentDataPath.Replace("/", "\\"))}\\GraphData\\player.acjson";
        if (!File.Exists(path))
        {
            File.Create(path);
        }
        string json = File.ReadAllText(path);

        var playerGraph = JsonUtility.FromJson<GraphData>(json);
        GraphEditorManager.Instance ??= new GraphEditorManager();
        GraphEditorManager.Instance.AjustAdditionalStatus(playerGraph.aditionalStatus);

        int stageIndex = StageSelector.stageIndex;

        SetWeaponCharacter(player, playerGraph);

        _playerExecutor = new GraphExecutor(playerGraph, player, enemy);

        player.ChangeSkin(playerGraph.skin);

        player.Start();
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
            weapon.user = chara;
        }
        else
        {
            weapon = null;
        }
        chara.weapon = weapon;
    }

    public void Reload()
    {
        if (player != null)
        {
            player.Death();
        }
        player = Instantiate(clonePlayer);
        player.gameObject.SetActive(true);
        if (enemy != null)
        {
            enemy.Death();
        }
        enemy = Instantiate(cloneEnemy);
        enemy.gameObject.SetActive(true);
        Start();
    }

    private void Awake()
    {
        clonePlayer = Instantiate(player);
        clonePlayer.gameObject.SetActive(false);
        cloneEnemy = Instantiate(enemy);
        cloneEnemy.gameObject.SetActive(false);
    }

    async void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
        Time.timeScale = 0;
        Application.targetFrameRate = 30;
        await Task.Delay(150);
        await InstanceExecutor();
        Time.timeScale = 1f;
        isRunning = true;
    }

    void Update()
    {
        if (!isRunning)
        {
            return;
        }

        if (player == null || enemy == null)
        {
            Reload();
            return;
        }

        _tickTimer += Time.deltaTime;

        if (_tickTimer >= TickInterval)
        {
            _tickTimer -= TickInterval;
            _playerExecutor.ExecuteTick();
        }
    }
}
