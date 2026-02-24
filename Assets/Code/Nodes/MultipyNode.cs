using System.Collections.Generic;
public class MultipyNode : Node
{
    public const string elementsPortName = "elements";
    public const string resultPortName = "Result";

    public override void Initialize()
    {
        nodeType = NodeType.Multiply;
        inputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
            new Port(elementsPortName, typeof(float), isRequired:true, isInput:true, isExecutionPort:false, this),
        };
        outputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:false, isInput:false, isExecutionPort:true, this),
            new Port(resultPortName, typeof(float), false, false, false, this),
        };
    }

    public override void Execute(GraphExecutor executor)
    {
        if (TryGetInputValuesWithPort<float>(elementsPortName, out List<float> elements))
        {
            float multi = 0f;
            foreach (var element in elements)
            {
                multi *= element;
            }
            executor.SendData(this, resultPortName, multi);
        }
        executor.EnqueueConnected(this, executePortName);
    }
}

