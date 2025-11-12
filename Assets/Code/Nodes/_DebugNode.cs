using System.Collections.Generic;

public class _DebugNode : Node
{
    public override void Initialize()
    {
        nodeType = NodeType.DEBUG;
        inputPorts = new Port[]
        {
            new("exec", typeof(bool), true, true, true, this),
            new("input", typeof(bool), false, true, false, this)
        };
        outputPorts = new Port[]
        {
            new("exec", typeof(bool), false, false, true, this)
        };
    }
    public override void Execute(GraphExecutor executor)
    {
        UnityEngine.Debug.Log($"発火ノードが実行されている:{inputValues.Count}");
        if (TryGetInputValueWithPort("input", out List<object> value))
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
