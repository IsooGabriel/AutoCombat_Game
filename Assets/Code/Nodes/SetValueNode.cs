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
            new Port("Execute", typeof(bool), false, false, true, this),
            new Port("output number", typeof(float), false, false, false, this),
        };
    }
    public override void Execute(GraphExecutor executor)
    {
        if (CheckValidence())
        {
            executor.SendData(this, outputPorts[1].portName, new List<float>() { (float)inputValues[0].value });
        }
        executor.EnqueueConnected(this, outputPorts[0].portName);
    }
    public bool CheckValidence(int index = 0)
    {
        if (inputValues == null)
        {
            return false;
        }
        if (inputValues.Count <= index)
        {
            return false;
        }
        if (inputValues[index] == null)
        {
            return false;
        }
        if (inputValues[index].value is float == false)
        {
            return false;
        }
        return true;
    }
}
