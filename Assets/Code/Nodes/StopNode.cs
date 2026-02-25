public class StopNode : Node
{

    public override void Initialize()
    {
        nodeType = NodeType.Stop;
        inputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
        };
        outputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:false, isExecutionPort:true, this),
        };
    }
    public override void Execute(GraphExecutor executor)
    {
        executor.myCharacter.Stop();
        executor.EnqueueConnected(this, executePortName);
    }
}

