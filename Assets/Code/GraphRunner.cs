using UnityEngine;

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


    private void InstanceExecutor()
    {
        var playerGraph = JsonUtility.FromJson<GraphData>(playerGraphJson.text);
        var enemyGraph = JsonUtility.FromJson<GraphData>(enemyGraphJson.text);
        if (playerGraph == null || enemyGraph == null || player == null || enemy == null)
        {
            Debug.LogError("なんかたんない");
            return;
        }
        _playerExecutor = new GraphExecutor(playerGraph, player, enemy);
        _enemyExecutor = new GraphExecutor(enemyGraph, enemy, player);
    }


    void Start()
    {
        Application.targetFrameRate = 30;

        InstanceExecutor();
    }

    void Update()
    {
        _tickTimer += Time.deltaTime;

        if (_tickTimer >= TickInterval)
        {
            _tickTimer -= TickInterval;
            _playerExecutor.ExecuteTick();
            _enemyExecutor.ExecuteTick();
        }
    }
}