using System.Collections.Generic;
public class SetValueNode : Node
{
    public override void Initialize()
    {
        nodeType = NodeType.SetValue;
        inputPorts = new Port[]
        {
            new Port("Execute", typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
        };
        outputPorts = new Port[]
        {
            new Port("output number", typeof(float), false, false, false, this),
        };
    }
    public override void Execute(GraphExecutor executor)
    {
        if (inputValues != null && inputValues.Count != 0)
        {
            TryGetInputValueWithPort(outputPorts[0].name, out List<float> values);
            executor.SendData(this, outputPorts[0].name, values);
        }
    }
}
