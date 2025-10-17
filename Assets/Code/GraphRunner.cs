using UnityEngine;

public class GraphRunner : MonoBehaviour
{
    public CharacterController character;
    public TextAsset graphJsonFile; // Inspectorで設定

    private GraphExecutor _executor;
    private float _tickTimer;
    private const float TickInterval = 1f / 30f; // 30tick/秒

    void Start()
    {
        // JSONからグラフ読み込み
        var graphData = JsonUtility.FromJson<GraphData>(graphJsonFile.text);
        _executor = new GraphExecutor(graphData, character);
    }

    void Update()
    {
        _tickTimer += Time.deltaTime;

        if (_tickTimer >= TickInterval)
        {
            _tickTimer -= TickInterval;
            _executor.ExecuteTick();
        }
    }
}