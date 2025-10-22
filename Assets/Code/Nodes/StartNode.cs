using System;
using static TreeEditor.TreeEditorHelper;

public class StartNode : Node
{
    public override void Initialize()
    {
        nodeType = NodeType.Start;
        inputPorts = new NodePort[0];
        outputPorts = new NodePort[] {new("exec", typeof(Type),true,false,true,this) };
    }

    public override void Execute(GraphExecutor executor)
    {
        executor.EnqueueConnected(this, "exec");
        executor.SendData(this, "exec", null);
    }
}