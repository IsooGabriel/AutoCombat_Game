
using System.Collections.Generic;
using System.Linq;

public static class NodeFactory
{
    private static Dictionary<NodeType, System.Func<Node>> _creators = new()
    {
        { NodeType.Start, () => new StartNode() },
        { NodeType.Move, () => new MoveNode() },
        { NodeType.Attack, () => new AttackNode() },
        //{ NodeType.Jump, () => new JumpNode() },
        //{ NodeType.GetDistance, () => new GetDistanceNode() },
        //{ NodeType.GetHP, () => new GetHPNode() },
        //{ NodeType.Compare, () => new CompareNode() },
        //{ NodeType.Branch, () => new BranchNode() },
        { NodeType.DEBUG, () => new _DebugNode() },
        { NodeType.SetValue, () => new SetValueNode() },
        { NodeType.SetVector, () => new SetVectorNode() },
        { NodeType.AND, () => new ANDNode() },
        { NodeType.IF, () => new IfNode() },
        { NodeType.BreakVector, () => new BreakVectorNode() },
        { NodeType.GetPosition, () => new GetPositionNode() },
        { NodeType.SetBool, () => new SetBoolNode() },
    };

    public static Node Create(NodeType type)
    {
        if (_creators.TryGetValue(type, out var creator))
        {
            return creator();
        }

        throw new System.ArgumentException($"Unknown node type: {type}");
    }

    // エディタで使うノードタイプ一覧(Startを除外)
    public static IEnumerable<NodeType> GetCreatableNodeTypes()
    {
        return System.Enum.GetValues(typeof(NodeType))
            .Cast<NodeType>()
            .Where(t => t != NodeType.Start && t != NodeType.DEBUG);
    }
}