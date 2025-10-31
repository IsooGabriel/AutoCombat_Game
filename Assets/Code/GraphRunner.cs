using UnityEngine;

public class GraphRunner : MonoBehaviour
{
    public Character player;
    public Character enemy;
    public TextAsset graphJsonFile; // Inspectorで設定

    private GraphExecutor _playerExecutor;
    private GraphExecutor _enemyExecutor;
    private float _tickTimer;
    private const float TickInterval = 1f / 30f; // 30tick/秒

    void Start()
    {

        Application.targetFrameRate = 30;

        // JSONからグラフ読み込み
        var graphData = JsonUtility.FromJson<GraphData>(graphJsonFile.text);
        if (graphData == null || player == null || enemy == null)
        {
            Debug.LogError("なんかたんない");
            return;
        }
        _playerExecutor = new GraphExecutor(graphData, player, enemy);
        _enemyExecutor = new GraphExecutor(graphData, enemy, player);
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