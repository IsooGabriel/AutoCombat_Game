using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StageData
{
    public string sceneName;
    public string scenePath;
    public Sprite thumbnail;
    public int stageNumber;
}

[CreateAssetMenu(fileName = "StageDatabase", menuName = "Game/Stage Database")]
public class StageDatabase : ScriptableObject
{
    public List<StageData> stages = new List<StageData>();
}