
using System.Collections.Generic;
using System.Linq;
using static NodeType;

public static class NodeFactory
{
    private static Dictionary<NodeType, System.Func<Node>> _creators = new()
    {
        { Start, () => new StartNode() },
        { Move, () => new MoveNode() },
        { Attack, () => new AttackNode() },
        //{ NodeType.Jump, () => new JumpNode() },
        //{ NodeType.GetDistance, () => new GetDistanceNode() },
        //{ NodeType.GetHP, () => new GetHPNode() },
        //{ NodeType.Compare, () => new CompareNode() },
        //{ NodeType.Branch, () => new BranchNode() },
        { DEBUG, () => new _DebugNode() },
        { SetValue, () => new SetValueNode() },
        { SetVector, () => new SetVectorNode() },
        { AND, () => new ANDNode() },
        { IF, () => new IfNode() },
        { BreakVector, () => new BreakVectorNode() },
        { GetPosition, () => new GetPositionNode() },
        { SetBool, () => new SetBoolNode() },
        { DoNTime, () => new DoNTimeNode() },
        { Sum, () => new SumNode()},
        { GetStatus, () => new GetStatusNode() },
        { Multiply, () => new MultipyNode() }, 
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