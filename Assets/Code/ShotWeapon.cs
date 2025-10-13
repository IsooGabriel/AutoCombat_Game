using System;
using UnityEngine;

public class ShotWeapon : Weapon
{
    public ShotWeapon(Entity user) : base(user)
    {
    }
    public override bool Attack(Vector2 direction)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(user.transform.position, direction, out hitInfo, (int)range))
        {
            Entity target = hitInfo.collider.GetComponent<Entity>();
            if (target != null)
            {
                Hit(target);
                target.TakeDamege(user.Attack);
                return true;
            }
        }
        return false;
    }
}