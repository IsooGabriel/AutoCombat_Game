using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public decimal damage;
    public float range;
    public float attackSpeed;
    public Character user;
    public Action<Character> hitAttack;

    private void SetDamage(Character target)
    {
        target.TakeDamage((int)damage);
        hitAttack?.Invoke(target);
    }
    public virtual void Attack(Character target)
    {
        SetDamage(target);
    }
}