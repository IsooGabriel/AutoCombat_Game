using System.Collections.Generic;

public class SumNode : Node
{
    public const string elementsPortName = "elements";
    public const string resultPortName = "Result";

    public override void Initialize()
    {
        nodeType = NodeType.Sum;
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
        if (TryGetInputValueWithPort<List<float>>(elementsPortName, out List<float> elements))
        {
            float sum = 0f;
            foreach (var element in elements)
            {
                sum += element;
            }
            executor.SendData(this, resultPortName, sum);
        }
        executor.EnqueueConnected(this, executePortName);
    }
}

