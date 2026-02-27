using System.Collections.Generic;
using System;
using UnityEngine;

public class StageCustomer : MonoBehaviour
{
    [SerializeField]
    private CustumStageSettings[] custums = { };
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject enemy;

    void Start()
    {
        string stageName = StageSelector.stageName;
        int stageIndex = StageSelector.stageIndex;
        foreach (var custum in custums)
        {
            custum.SelectActive(stageName, stageIndex, player, enemy);
        }
    }
}
[Serializable]      
public class CustumStageSettings
{
    [SerializeField]
    private string targetStageName;
    [SerializeField]
    private int targetStageIndex;
    [SerializeField]
    private CustumSettings custum;
    [SerializeField]
    private Vector2 playerPosition = new Vector2(-4f, 2.5f);
    [SerializeField]
    private Vector2 enemyPosition = new Vector2(4f, 2.5f);

    public void SelectActive(string stageName, int stageindex, GameObject player, GameObject enemy)
    {
        bool isTarget = (stageName == targetStageName || stageindex == targetStageIndex);
        if(!isTarget)
        {
            return;
        }
        custum.SelectActive(isTarget);
        player.transform.position = playerPosition;
        enemy.transform.position = enemyPosition;
    }
}