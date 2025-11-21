using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GraphRunner : MonoBehaviour
{
    public Character player;
    public Character enemy;
    public TextAsset playerGraphJson;
    public TextAsset enemyGraphJson;

    private GraphExecutor _playerExecutor;
    private GraphExecutor _enemyExecutor;
    private float _tickTimer;
    private const float TickInterval = 1f / 30f; // 30tick/秒

    private bool isRunning = false;

    private void InstanceExecutor()
    {
        string path = Path.Combine(Application.persistentDataPath, "PlaeyreData.json");
        string json = File.ReadAllText(path);

        string enemypath = Path.Combine(Application.persistentDataPath, "EnemyData.json");
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
        SceneManager.LoadScene("GraphEditor");
    }
    private void OnEnemyWin()
    {
        SceneManager.LoadScene("GraphEditor");
    }


    void Start()
    {
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