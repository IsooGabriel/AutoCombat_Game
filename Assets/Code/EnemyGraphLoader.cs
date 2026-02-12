using System.IO;
using UnityEngine;

public class EnemyGraphLoader : MonoBehaviour
{
    public const string graphDirectory = "EnemyGraph";
    public const string graphFile = "enemy";
    public const string graphExtension = ".json";
    public const string graphPath = graphDirectory + "\\" + graphFile + graphExtension;


    void Start()
    {
        if (!Directory.Exists(Application.persistentDataPath.Replace("/", "\\") + $"\\{graphDirectory}"))
        {
            Directory.CreateDirectory(Application.persistentDataPath.Replace("/", "\\") + $"\\{graphDirectory}");
        }

        string path = Application.persistentDataPath.Replace("/", "\\") + $"\\{graphPath}";
        if (!File.Exists(path))
        {
            TextAsset json = Resources.Load<TextAsset>(graphDirectory + "/" + graphFile);

            string data = json?.text;

            File.WriteAllText(path, data, System.Text.Encoding.UTF8);
        }
    }
}
