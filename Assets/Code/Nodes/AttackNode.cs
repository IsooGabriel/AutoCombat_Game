using System;
using UnityEngine;

public class AttackNode : Node
{
    readonly public string inputDirection = "direction";
    readonly public string inputAutoAim = "isAutoAim";
    readonly public string outOnHit = "onHit";

    private GraphExecutor executor;
    private Action<Character> onHit;

    private bool isAutoAim
    {
        get
        {
            if (TryGetInputValueWithPort(inputAutoAim, out bool value))
            {
                return value;
            }
            return true;
        }
    }

    public override void Initialize()
    {
        nodeType = NodeType.Attack;
        inputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
            new Port(inputDirection, typeof(Vector2), isRequired:false, isInput:true, isExecutionPort:false, this),
            new Port(inputAutoAim, typeof(bool), isRequired:false, isInput:true, isExecutionPort:false, this),
        };
        outputPorts = new Port[]
        {
            new Port(executePortName, typeof(bool), false, false, true, this),
            new Port(outOnHit, typeof(bool), false, false, true, this)
        };
        onHit = OnHitAttack;
    }
    public override void Execute(GraphExecutor exector)
    {
        executor = exector;
        Vector2 direction;

        if (isAutoAim)
        {
            exector.myCharacter.Attack(exector, exector.enemy.transform, onHit);
        }
        else
        {
            if (!TryGetInputValueWithPort(inputDirection, out direction))
            {
                if (!TryGetInputValueWithPort(inputDirection, out Vector3 vector3))
                {
                    return;
                }
                direction = new Vector2(vector3.x, vector3.y);
            }

            exector.myCharacter.Attack(exector, direction, onHit);
        }
        exector.EnqueueConnected(this, executePortName);
    }

    private void OnHitAttack(Character target)
    {
        executor.EnqueueConnected(this, outOnHit);
    }
}
