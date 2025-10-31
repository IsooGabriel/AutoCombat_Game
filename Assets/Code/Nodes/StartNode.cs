using System;

public class StartNode : Node
{
    public override void Initialize()
    {
        nodeType = NodeType.Start;
        inputPorts = new Port[0];
        outputPorts = new Port[] { new("exec", typeof(Type), true, false, true, this) };
    }

    public override void Execute(GraphExecutor executor)
    {
        executor.EnqueueConnected(this, "exec");
        executor.SendData(this, "exec", null);
    }
}