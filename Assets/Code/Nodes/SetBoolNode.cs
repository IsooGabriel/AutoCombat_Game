
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class SetBoolNode : Node
{
    public const string inputBool = "inputBool";
    readonly string outBool = "outBool";
    public override void Initialize()
    {
        nodeType = NodeType.SetBool;
        inputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
        };
        outputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), false, false, true, this),
            new Port(outBool, typeof(bool), false, false, false, this),
        };
    }
    public override void Execute(GraphExecutor executor)
    {
        float binary = 1;
        bool value = true;
        if(!TryGetInputValueWithPort(inputBool, out binary))
        {
            value = true;
        }
        else
        {
            value = binary != 0;
        }
        executor.SendData(this, outBool, value);
        executor.EnqueueConnected(this, outputPorts[0].portName);
    }
}

