using UnityEngine;
public class GetPositionNode : Node
{
    public const string positionTypeDataName = "isCharacterPostion";
    private string positionPortName = "position";
    private GetPositionSettings isCharacterPostion = GetPositionSettings.CharacterPosition;
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
        if(TryGetInputValueWithPort(positionTypeDataName, out float newValue))
        {
            isCharacterPostion = (GetPositionSettings)(int)newValue;
        }


        Vector2 send = Vector2.zero;
        switch (isCharacterPostion)
        {
            case GetPositionSettings.CharacterPosition:
                send = executor.myCharacter.transform.position;
                break;
            case GetPositionSettings.EnemyPosition:
                send = executor.enemy.transform.position;
                break;
            default:
                break;
        }
                executor.SendData(this, positionPortName, (object)new Vector2(send.x, send.y));
        executor.EnqueueConnected(this, executePortName);
    }
}
