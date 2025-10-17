using System;

public class _DebugNode : Node
{
    public override void Initialize()
    {
        nodeType = "_Debug";
        inputPorts = new NodePort[]
        {
            new("exec", typeof(object), true, true, true, this),
            new("input", typeof(object), true, true, false, this)
        };
        outputPorts = new NodePort[]
        {
            new("exec", typeof(Type), true, false, true, this)
        };
    }
    public override void Execute(GraphExecutor executor)
    {
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
