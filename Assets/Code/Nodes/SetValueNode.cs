using System.Collections.Generic;
public class SetValueNode : Node
{

    const string outputValuePortName = "output number";

    const string outputValuePortNameJP = "値";
    public override void Initialize()
    {
        nodeType = NodeType.SetValue;
        useLimit = 99;

        nameToJP.Add(outputValuePortName, outputValuePortNameJP);

        inputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
        };
        outputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), false, false, true, this),
            new Port(outputValuePortName, typeof(float), false, false, false, this),
        };
    }
    public override void Execute(GraphExecutor executor)
    {
        if (CheckValidence())
        {
            object val = inputValues[0].value;
            // 数値型ならfloatに変換して送信（現在のポート定義に合わせる）
            if (val is float f)
            {
                executor.SendData(this, outputValuePortName, f);
            }
            else if (val is int i)
            {
                executor.SendData(this, outputValuePortName, (float)i);
            }
            else
            {
                executor.SendData(this, outputValuePortName, val);
            }
        }
        executor.EnqueueConnected(this, executePortName);
    }
    public bool CheckValidence(int index = 0)
    {
        if (inputValues == null || inputValues.Count <= index || inputValues[index] == null)
        {
            return false;
        }
        
        object val = inputValues[index].value;
        return val is float || val is int || val is double || val is SerializedValue;
    }
}
