using UnityEngine;
public class GameManager
{
    private static GameManager _instance;
    private static readonly object _lock = new object();
    private GameManager() { }

    public static GameManager Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new GameManager();
                }
                return _instance;
            }
        }
    }

    public Entity Player { get; set; }
    public Entity Enemy { get; set; }
    [SerializeField]
    private GameObject winUI;
    [SerializeField]
    private GameObject loseUI;
    public void Run()
    {
        if (Player == null || Enemy == null)
        {
            Debug.LogError("Player or Enemy is not set!");
            return;
        }
        Player.Run();
        Enemy.Run();
    }
    public void EntityDie(Entity entity)
    {
        if (entity == Player)
        {
            if (loseUI != null)
            {
                loseUI.SetActive(true);
            }
        }
        else if (entity == Enemy)
        {
            if (winUI != null)
            {
                winUI.SetActive(true);
            }
        }
    }
}