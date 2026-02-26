using UnityEngine;
public class BreakVectorNode : Node
{
    readonly string toVectorPort = "vector";
    readonly string fromXPort = "x";
    readonly string fromYPort = "y";

    public override void Initialize()
    {
        nodeType = NodeType.BreakVector;
        useLimit = 99;
        inputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
            new Port(toVectorPort, typeof(Vector2), isRequired:true, isInput:true, isExecutionPort:false, this),
        };
        outputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), false, false, true, this),
            new Port(fromXPort, typeof(float), false, false, false, this),
            new Port(fromYPort, typeof(float), false, false, false, this),
        };
    }
    public override void Execute(GraphExecutor executor)
    {
        if(!TryGetInputValueWithPort(toVectorPort, out Vector2 vector))
        {
            if(!TryGetInputValueWithPort(toVectorPort, out Vector3 vector3))
            {
                return;
            }
            vector = vector3;
        }
        float x = vector.x;
        float y = vector.y;
        executor.SendData(this, fromXPort, x);
        executor.SendData(this, fromYPort, y);
        executor.EnqueueConnected(this, executePortName);
    }
}
