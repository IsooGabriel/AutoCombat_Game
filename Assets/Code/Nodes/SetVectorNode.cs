using System.Collections.Generic;
using UnityEngine;

public class SetVectorNode : Node
{
    const string portXname = "x";
    const string portYname = "y";
    public override void Initialize()
    {
        nodeType = NodeType.SetVector;
        inputPorts = new Port[]
        {
            new Port("Execute", typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
            new Port(portXname, typeof(float), isRequired:false, isInput:true, isExecutionPort:false, this),
            new Port(portYname, typeof(float), isRequired:false, isInput:true, isExecutionPort:false, this),
        };
        outputPorts = new Port[]
        {
            new Port("Execute", typeof(bool), false, false, true, this),
            new Port("output vector2", typeof(Vector2), false, false, false, this),
        };
    }

    public override void Execute(GraphExecutor executor)
    {
        if (TryGetInputValueWithPort(portXname, out List<float> valuesX) &&
            TryGetInputValueWithPort(portYname, out List<float> valuesY))
        {
            executor.SendData(this, outputPorts[1].portName, new List<Vector2>() { new Vector2(valuesX[0], valuesY[0]) });
        }
        executor.EnqueueConnected(this, outputPorts[0].portName);
    }
}
