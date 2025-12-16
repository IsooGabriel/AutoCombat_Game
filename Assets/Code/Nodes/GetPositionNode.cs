using UnityEngine;
using System.Collections.Generic;
public class GetPositionNode : Node
{
    public const string positionTypeDataName = "isCharacterPostion";
    private string positionPortName = "position";
    public override void Initialize()
    {
        nodeType = NodeType.GetPosition;
        inputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
        };
        outputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:false, isInput:false, isExecutionPort:true, this),
            new Port(positionPortName, typeof(Vector2), false, false, false, this),
        };
    }

    public override void Execute(GraphExecutor executor)
    {
        if (!TryGetInputValueWithPort(positionTypeDataName, out float isCharacterPostion))
        {
            return;
        }

        switch ((GetPositionSettings)isCharacterPostion)
        {
            case GetPositionSettings.CharacterPosition:
                executor.SendData(this, positionPortName, (object)executor.myCharacter.transform.position);
                break;
            case GetPositionSettings.EnemyPosition:
                executor.SendData(this, positionPortName, (object)executor.enemy.transform.position);
                break;
            default:
                break;
        }
        executor.EnqueueConnected(this, executePortName);
    }
}
