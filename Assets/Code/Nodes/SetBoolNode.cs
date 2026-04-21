public class SetBoolNode : Node
{
    public const string inputBool = "inputBool";
    readonly string outBool = "outBool";

    public const string inputBoolJP = "inputBool";
    public const string outBoolJP = "値";

    public override void Initialize()
    {
        nodeType = NodeType.SetBool;
        useLimit = 99;

        nameToJP.Add(inputBool, inputBoolJP);
        nameToJP.Add(outBool, outBoolJP);

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
        bool value = true;
        if (TryGetInputValueWithPort(inputBool, out bool boolVal))
        {
            value = boolVal;
        }
        else if (TryGetInputValueWithPort(inputBool, out float floatVal))
        {
            value = floatVal != 0;
        }
        else if (TryGetInputValueWithPort(inputBool, out int intVal))
        {
            value = intVal != 0;
        }

        executor.SendData(this, outBool, value);
        executor.EnqueueConnected(this, outputPorts[0].portName);
    }
}

