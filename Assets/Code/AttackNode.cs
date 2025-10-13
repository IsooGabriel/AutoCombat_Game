using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackNode : INode
{
    [SerializeField]
    private Entity user;

    public IEnumerable<Port> InputPorts { get; private set; }
    public IEnumerable<Port> OutputPorts { get; private set; }
    public AttackNode()
    {
        InputPorts = new List<Port> { new Port("TryAttack", typeof(bool), PortDirection.Input, this), new Port("direction", typeof(Vector2), PortDirection.Input, this) };
        OutputPorts = new List<Port> { new Port("hit", typeof(bool), PortDirection.Output, this) };
    }

    public void Execute(NodeContext context)
    {
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

        //if ( && dirObj is Vector2 dir)
        //{
        //    direction = dir;
        //}
        if (user == GameManager.Instance.Player)
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
            user.AttackTarget(user.transform.right);
    }
}