using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackNode : INode
{
    private Entity user;

    public IEnumerable<Port> InputPorts { get; private set; }
    public IEnumerable<Port> OutputPorts { get; private set; }
    public NodeContext contextLog { get; private set; }
    public AttackNode()
    {
        InputPorts = new List<Port> {new Port("setAttack", typeof(bool), PortDirection.Input, this, true), new Port("direction", typeof(Vector2), PortDirection.Input, this) };
        OutputPorts = new List<Port> { new Port("conti", typeof(bool), PortDirection.Output, this), new Port("hit", typeof(bool), PortDirection.Output, this) };
    }

    public void Execute(NodeContext context)
    {
        if (!INode.CheckExecutable(InputPorts, context))
        {
            return;
        }
        if (user == null)
        {
            Debug.LogError("User entity is not set.");
            return;
        }
        else if (GameManager.Instance.Player == null || GameManager.Instance.Enemy == null)
        {
            return;
        }
        Vector2 direction = context.GetValue<Vector2>(InputPorts.ElementAt(1));
        if(contextLog.TryGetValue(out direction))
        {
        }
        else if (user == GameManager.Instance.Player)
        {
            direction = GameManager.Instance.Enemy.transform.position;
        }
        else if (user == GameManager.Instance.Enemy)
        {
            direction = GameManager.Instance.Player.transform.position;
        }
        else
        {
            return;
        }
        user.AttackTarget(direction);
        user.Weapon.hit += OnHit;
    }
    private void OnHit(Entity other)
    {
        INode.Output(true, this);
    }
}