using UnityEngine;
public class AcceleNode : Node
{
    public const string fripPortName = "isEnemyFrip";
    public const string directionPortName = "direction";
    public const string newPositionPortName = "newPosition";

    public const string fripPortNameJP = "敵は左右逆転";
    public const string directionPortNameJP = "方向";
    public const string newPositionPortNameJP = "現在座標";



    public override void Initialize()
    {
        nodeType = NodeType.Accele;

        nameToJP.Add(fripPortName, fripPortNameJP);
        nameToJP.Add(directionPortName, directionPortNameJP);
        nameToJP.Add(newPositionPortName, newPositionPortNameJP);

        inputPorts = new Port[]
        {
            new Port(executePortName,  typeof(bool), isRequired:true, isInput:true, isExecutionPort:true, this),
            new Port(directionPortName,  typeof(Vector2), true, true, false, this),
            new Port(fripPortName, typeof(bool), false, true, false, this),
        };
        outputPorts = new Port[]
        {
            new Port(executePortName,   typeof(bool), isRequired:true, isInput:false, isExecutionPort:true, this),
            new Port(newPositionPortName,  typeof(Vector2), true, false, false, this),
        };
    }
    public override void Execute(GraphExecutor executor)
    {
        if (!InputValueContainsPort(directionPortName))
        {
            return;
        }
        Vector2 directions = new() { };
        Vector2 direction = TryGetInputValueWithPort(directionPortName, out directions) == true ? directions : Vector2.zero;
        if (TryGetInputValueWithPort(fripPortName, out bool flip) && flip && !executor.myCharacter.isPlayer)
        {
            direction.x *= -1;
        }
        executor.myCharacter.Accele(direction);
        executor.EnqueueConnected(this, executePortName);
    }
}

