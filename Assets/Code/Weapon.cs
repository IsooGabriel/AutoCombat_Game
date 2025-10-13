using System;
using UnityEngine;
public class Weapon
{
    protected Entity user;

    [SerializeField]
    protected decimal range = 1;
    public Action<Entity> hit;

    public Weapon(Entity user)
    {
        this.user = user;
    }

    public virtual bool Attack(Vector2 direction)
    {
        return false;
    }

    public void Hit(Entity target)
    {
        target.TakeDamege(user.Attack);
        hit?.Invoke(target);
    }
}