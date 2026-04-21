using System;

public class ANDNode : LinkedNode
{
    const string elementPortName = "element";
    const string resultPortName = "result";

    const string elementPortNameJP = "値";
    const string resultPortNameJP = "結果";

    private Type connectionType = typeof(float);
    private bool firstExecution = true;
    private int executionCount = 0;

    public override void Initialize()
    {
        useLimit = -1;
        nodeType = NodeType.AND;

        nameToJP.Add(elementPortName, elementPortNameJP);
        nameToJP.Add(resultPortName, resultPortNameJP);

        inputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
            new Port(elementPortName, typeof(object), isRequired:false, isInput:true, isExecutionPort:false, this),
        };
        outputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), false, false, true, this),
            new Port(resultPortName, typeof(object), false, false, false, this),
        };
    }

    public override void Execute(GraphExecutor executor)
    {
        if (firstExecution)
        {
            connectionType = GetConnectionType();
            firstExecution = false;
        }
        executionCount++;
        if (executionCount < toNodes.Length)
        {
            return;
        }

        executor.EnqueueConnected(this, outputPorts[0].portName);
    }

    public override void FlameInitialize()
    {
        executionCount = 0;
    }
}
