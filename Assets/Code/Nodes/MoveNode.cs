using System.Linq;
using UnityEngine;

public class MoveNode : Node
{
    public override void Initialize()
    {
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
        if (!inputData.ContainsKey("direction"))
        {
            return;
        }
        Vector2 direction = (Vector2)inputData["direction"].FirstOrDefault(x => x is Vector2);
        executor.myCharacter.Move(direction);
    }
}
