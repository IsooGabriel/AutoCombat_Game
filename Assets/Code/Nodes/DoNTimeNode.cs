
public class DoNTimeNode : Node
{
    public const string limitPortName = "Limit";
    public const string nowLimitPortName = "NowLimit";
    public const string limitedPortName = "whithLimit";
    private int useLimit = 0;
    public override void Initialize()
    {
        nodeType = NodeType.DoNTime;
        inputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
        };
        outputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:false, isInput:false, isExecutionPort:true, this),
            new Port(nowLimitPortName, typeof(float), false, false, false, this),
            new Port(limitedPortName, typeof(bool), isRequired:false, isInput:false, isExecutionPort:true, this),

        };
    }

    public override void StartInitialize()
    {
        if (TryGetInputValueWithPort<float>(limitPortName, out float value))
        {
            useLimit = (int)value;
        }
    }

    public override void Execute(GraphExecutor executor)
    {
        if (useLimit > 0)
        {
            useLimit--;
            executor.EnqueueConnected(this, executePortName);
        }
        else
        {
            executor.EnqueueConnected(this, limitedPortName);
        }
        executor.SendData(this, nowLimitPortName, useLimit);
        return;
    }
}
