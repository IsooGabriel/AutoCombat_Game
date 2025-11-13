using System.Collections.Generic;
using UnityEngine;

public class SetVectorNode : Node
{
    public override void Initialize()
    {
        nodeType = NodeType.SetVector;
        inputPorts = new Port[]
        {
            new Port("Execute", typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
        };
        outputPorts = new Port[]
        {
            new Port("output vector2", typeof(Vector2), false, false, false, this),
        };
    }

    public override void Execute(GraphExecutor executor)
    {
        if (inputValues.Count < 2)
        {
            return;
        }
        TryGetInputValueWithPort(outputPorts[0].name, out List<float> values);
        executor.SendData(this, outputPorts[1].name, GenerateVectors(values));
    }

    private List<Vector2> GenerateVectors(List<float> values)
    {
        List<Vector2> vectors = new List<Vector2>();
        for (int i = 0; i < values.Count; i += 2)
        {
            if (i + 1 < values.Count)
            {
                vectors.Add(new Vector2(values[i], values[i + 1]));
            }
            else if (i < values.Count)
            {
                vectors.Add(new Vector2(values[i], 0f));
            }
        }
        return vectors;
    }
}
