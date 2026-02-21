using UnityEngine;
public class DoNTimeNode : Node
{
    public const string limitPortName = "Limit";
    public const string resetLimitPortName = "ResetLimit";
    public const string nowLimitPortName = "NowLimit";
    public const string limitedPortName = "whithLimit";
    private int _useLimit = 0;
    private int _resetLimit = 0;
    public override void Initialize()
    {
        useLimit = 2;
        nodeType = NodeType.DoNTime;
        inputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:false, isInput:true, isExecutionPort:true, this),
            new Port(resetLimitPortName, typeof(bool), isRequired:false, isInput:true, isExecutionPort:true, this)
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
            _useLimit = (int)value;
            _resetLimit = (int)value;
        }
    }

    public override void Execute(GraphExecutor executor)
    {
        if (TryGetInputValueWithPort(resetLimitPortName, out bool b))
        {
            _useLimit = _resetLimit;
        }
        if (_useLimit > 0)
        {
            _useLimit--;
            executor.EnqueueConnected(this, executePortName);
        }
        else
        {
            executor.EnqueueConnected(this, limitedPortName);
        }

        executor.SendData(this, nowLimitPortName, (float)_useLimit);
        return;
    }
}
