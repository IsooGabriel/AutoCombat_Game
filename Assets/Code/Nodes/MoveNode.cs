using System.Collections.Generic;
using UnityEngine;

public class MoveNode : Node
{

    public const string fripPortName = "isEnemyFrip";
    public const string directionPortName = "direction";
    public const string newPositionPortName = "newPosition";

    public override void Initialize()
    {
        nodeType = NodeType.Move;
        inputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
            new Port(directionPortName, typeof(Vector2), true, true, false, this),
            new Port(fripPortName, typeof(bool), false, true, false, this),
        };
        outputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:false, isExecutionPort:true, this),
            new Port(newPositionPortName, typeof(Vector2), true, false, false, this),
        };
    }
    public override void Execute(GraphExecutor executor)
    {
        if (!InputValueContainsPort(directionPortName))
        {
            return;
        }
        Vector2 directions = new() { };
        Vector2 direction = TryGetInputValueWithPort(directionPortName, out directions) == true ? directions : Vector2.zero;
        if (TryGetInputValueWithPort(fripPortName, out bool flip) && flip && !executor.myCharacter.isPlayer)
        {
            direction.x *= -1;
        }
        executor.myCharacter.Move(direction);
        executor.EnqueueConnected(this, executePortName);
    }
}
