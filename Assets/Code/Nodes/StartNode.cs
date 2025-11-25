public class StartNode : Node
{
    public override void Initialize()
    {
        nodeType = NodeType.Start;
        inputPorts = new Port[0];
        outputPorts = new Port[] { new(executePortName, typeof(bool), true, false, true, this) };
    }

    public override void Execute(GraphExecutor executor)
    {
        executor.EnqueueConnected(this, executePortName);
        executor.SendData(this, executePortName, null);
    }
}