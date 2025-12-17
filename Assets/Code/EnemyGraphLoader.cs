using System.IO;
using UnityEngine;

public class EnemyGraphLoader : MonoBehaviour
{
    public const string enemyGraphDir = "EnemyGraph";
    public const string enemyGraphPath = enemyGraphDir+"\\enemy.acjson";

    void Start()
    {
        if (!Directory.Exists(Application.persistentDataPath.Replace("/", "\\") + $"\\{enemyGraphDir}"))
        {
            Directory.CreateDirectory(Application.persistentDataPath.Replace("/", "\\") + $"\\{enemyGraphDir}");
        }

        TextAsset json = Resources.Load<TextAsset>(enemyGraphPath);

        string data = json.text;
        string path = Application.persistentDataPath.Replace("/", "\\")+ $"\\{enemyGraphPath}";

        File.WriteAllText(path, data, System.Text.Encoding.UTF8);
    }
}
