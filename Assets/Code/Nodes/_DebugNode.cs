using System;
using System.Diagnostics;

public class _DebugNode : Node
{
    public override void Initialize()
    {
        nodeType = NodeType.DEBUG;
        inputPorts = new Port[]
        {
            new("exec", typeof(object), true, true, true, this),
            new("input", typeof(object), false, true, false, this)
        };
        outputPorts = new Port[]
        {
            new("exec", typeof(Type), false, false, true, this)
        };
    }
    public override void Execute(GraphExecutor executor)
    {
        UnityEngine.Debug.Log($"発火ノードが実行されている:{inputData.Count}");
        if (inputData.TryGetValue("input", out object value))
        {
            UnityEngine.Debug.Log($"_DebugNode: {value}");
        }
        else
        {
            UnityEngine.Debug.Log("_DebugNode: No input data");
        }
        executor.EnqueueConnected(this, "exec");

#if !UNITY_EDITOR
        Form form = new Form();
        form.Text = "Warning";
        form.Width = 300;
        form.Height = 200;
        form.Show();
#endif
    }
}
