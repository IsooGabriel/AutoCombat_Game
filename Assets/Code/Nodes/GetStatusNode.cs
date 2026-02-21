using UnityEngine;
using static StatusType;
public enum StatusType
{
    MAX_HP = 1,
    CURRENT_HP,
    ATTACK,
    ATTACK_CT,
    CRITICAL_CHANCE,
    CRITICAL_DAMAGE,
}
public class GetStatusNode:Node
{

    public const string targetPortName = "enemy is target";
    public const string statusPortName = "source";
    public const string returnPortName = "result";

    private bool isTargetPlayer = true;
    private float targetStartsu = 0f;

    public override void Initialize()
    {
        nodeType = NodeType.GetStatus;
        inputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
            new Port(targetPortName, typeof(bool), isRequired:true,isInput:true, isExecutionPort:false, this),
        };
        outputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:false, isInput:false, isExecutionPort:true, this),
            new Port(returnPortName, typeof(float), false, false, false, this)
        };
    }

    public override void Execute(GraphExecutor executor)
    {
        if(!TryGetInputValueWithPort<bool>(targetPortName, out isTargetPlayer))
        {
            isTargetPlayer = true;
        }
        if (!TryGetInputValueWithPort<float>(statusPortName, out targetStartsu))
        {
            return;
        }
        var chara = isTargetPlayer ? executor.myCharacter : executor.enemy;

        object state = (StatusType)Mathf.RoundToInt(targetStartsu) switch
        {
            MAX_HP => chara.statsu.hp,
            CURRENT_HP => chara.currentHP,
            ATTACK => chara.statsu.attack,
            ATTACK_CT => chara.statsu.attackCooltime,
            CRITICAL_CHANCE => chara.statsu.criticalChance,
            CRITICAL_DAMAGE => chara.statsu.criticalDamage,
            _ => null,
        };
        executor.SendData(this, returnPortName, state);
        executor.EnqueueConnected(this, executePortName);
    }
}