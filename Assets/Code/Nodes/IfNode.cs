using System.Threading.Tasks;

public class IfNode : Node
{
    private IfSettings ifSettings;
    private bool result;

    private readonly string valueAPortName = "value A";
    private readonly string valueBPortName = "value B";
    private const string truePortName = "True";
    private const string falsePortName = "False";

    private readonly string valueAPortNameJP = "ílA";
    private readonly string valueBPortNameJP = "ílB";
    private const string truePortNameJP = "ê^";
    private const string falsePortNameJP = "ãU";

    public override void Initialize()
    {
        nodeType = NodeType.IF;
        useLimit = 99;

        nameToJP.Add(valueAPortName, valueAPortNameJP);
        nameToJP.Add(valueBPortName, valueBPortNameJP);
        nameToJP.Add(truePortName, truePortNameJP);
        nameToJP.Add(falsePortName, falsePortNameJP);

        inputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
            new Port(valueAPortName, typeof(float), isRequired:true, isInput:true, isExecutionPort:false, this),
            new Port(valueBPortName, typeof(float), isRequired:true, isInput:true, isExecutionPort:false, this),
        };
        outputPorts = new Port[]
        {
            new Port(truePortName, typeof(bool), false, false, true, this),
            new Port(falsePortName, typeof(bool), false, false, true, this),
        };
    }

    public override void Execute(GraphExecutor executor)
    {
        if (!TryGetInputValueWithPort<IfSettings>(IfNodeUI.settingKey, out ifSettings))
        {
            if (TryGetInputValueWithPort<float>(IfNodeUI.settingKey, out float settingInt))
            {
                ifSettings = (IfSettings)settingInt;
            }
            else
            {
                return;
            }
        }
        float valueA = TryGetInputValueWithPort<float>(valueAPortName, out float valuesA) ? valuesA : 0f;
        float valueB = TryGetInputValueWithPort<float>(valueBPortName, out float valuesB) ? valuesB : 0f;
        switch (ifSettings)
        {
            case IfSettings.Equals:
                result = valueA == valueB;
                break;
            case IfSettings.NotEquals:
                result = valueA != valueB;
                break;
            case IfSettings.GreaterThan:
                result = valueA > valueB;
                break;
            case IfSettings.LessThan:
                result = valueA < valueB;
                break;
            default:
                break;
        }

        if (result)
        {
            executor.EnqueueConnected(this, outputPorts[0].portName);
        }
        else
        {
            executor.EnqueueConnected(this, outputPorts[1].portName);
        }
    }
}