public class IfNode : Node
{
    private IfSettings ifSettings;
    private bool result;

    private readonly string valueAPortName = "value A";
    private readonly string valueBPortName = "value B";

    public override void Initialize()
    {
        nodeType = NodeType.IF;
        inputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
            new Port("value A", typeof(float), isRequired:true, isInput:true, isExecutionPort:true, this),
            new Port("value B", typeof(float), isRequired:true, isInput:true, isExecutionPort:false, this),
        };
        outputPorts = new Port[]
        {
            new Port("True", typeof(bool), false, false, true, this),
            new Port("False", typeof(bool), false, false, true, this),
        };
    }

    public override void Execute(GraphExecutor executor)
    {
        if (TryGetInputValueWithPort<IfSettings>(IfNodeUI.settingKey, out ifSettings))
        {
        }
        else
        {
            return;
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

        if(result)
        {
            executor.EnqueueConnected(this, outputPorts[0].name);
        }
        else
        {
            executor.EnqueueConnected(this, outputPorts[1].name);
        }
    }
}