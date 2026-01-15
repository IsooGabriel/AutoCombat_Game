using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public decimal damage
    {
        get
        {
            decimal baseDamage = user != null ? user.baseStatus.attack + user.aditionalStatus.attack : 1;
            return baseDamage * (decimal)damageMultiply;
        }
    }
    public float damageMultiply = 1;
    public float range;
    public float attackSpeed;
    public Character user;
    public Action<Character> hitAttack;

    public float timer = 0;

    private void SetDamage(Character target)
    {
        target.TakeDamage((int)damage);
        hitAttack?.Invoke(target);
    }
    public virtual bool TryAttack(Vector2 direction)
    {
        if (timer > 0)
        {
            return false;
        }
        timer = attackSpeed;
        Ray ray = new Ray(transform.position, direction);
        int layerMask = 0;
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, range, layerMask); ;
        Debug.DrawRay(transform.position, direction, Color.red);
        if (hit.collider)
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null && target != user)
            {
                target.TakeDamage((int)damage);
            }
        }
        return true;
    }
    public virtual void Attack(Transform target)
    {
        TryAttack(target.position - transform.position);
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }
}