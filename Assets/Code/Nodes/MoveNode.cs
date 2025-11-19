using System.Collections.Generic;
using UnityEngine;

public class MoveNode : Node
{
    public override void Initialize()
    {
        nodeType = NodeType.Move;
        inputPorts = new Port[]
        {
            new Port("Execute", typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
            new Port("direction", typeof(Vector2), true, true, false, this),
        };
        outputPorts = new Port[]
        {
            new Port("Execute", typeof(bool), isRequired:true, isInput:false, isExecutionPort:true, this),
            new Port("newPosition", typeof(Vector2), true, false, false, this),
        };
    }
    public override void Execute(GraphExecutor executor)
    {
        if (!InputValueContainsPort("direction"))
        {
            return;
        }
        List<Vector2> directions = new() { };
       Vector2 direction = TryGetInputValueWithPort("direction", out directions) == true ? directions[0] : Vector2.zero;
        executor.myCharacter.Move(direction);
    }
}
