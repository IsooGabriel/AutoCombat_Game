using System;
using System.Numerics;
using static UnityEngine.GraphicsBuffer;

public class Weapon
{
    private Entity user;

    public Action<Entity> hit;

    public Weapon(Entity user)
    {
        this.user = user;
    }

    public virtual bool Attack(Vector3 direction)
    {
        return true;
    }

    public void Hit(Entity target)
    {
        target.TakeDamege(user.Attack);
        hit?.Invoke(target);
    }
}