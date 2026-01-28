using System;
using UnityEngine;

public interface IDeathable
{
    public void Death();
}
public interface IDamageable
{
    public void TakeDamage(int damage);
}

public class Character : MonoBehaviour, IDeathable, IDamageable
{
    public Status baseStatus { get; set; } = new() { hp = 10, attack = 1, attackCooltime = 1, criticalChance = 20, criticalDamage = 40 };
    public Status aditionalStatus { get; set; } = new() { hp = 0, attack = 0, attackCooltime = 0, criticalChance = 0, criticalDamage = 0 };
    public int currentHP = 10;
    const decimal speed = 5;
    public Weapon.Weapon weapon;
    public Action<Character> takeDamage;
    public Action<Character> onDeath;
    public bool isPlayer = false;

    public void Death()
    {
        onDeath?.Invoke(this);
        Destroy(this.gameObject);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        takeDamage?.Invoke(this);
        if (currentHP <= 0)
        {
            Death();
        }
    }

    public Vector2 Move(Vector2 direction)
    {
        direction.Normalize();
        float multiply = (float)speed * Time.deltaTime;
        direction = new Vector2(direction.x * multiply, direction.y * multiply);

        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.AddForce(direction * 75, ForceMode2D.Force);
        }
        else
        {
            transform.position += (Vector3)direction;
        }
        return transform.position;
    }

    public void Attack(GraphExecutor exector, Vector3 direction, Action<Character> onHit)
    {
        weapon.TryAttack(direction);
        weapon.hitAttack += onHit;
    }
    public Action<Character> Attack(GraphExecutor exector, Vector2 direction)
    {
        Attack(exector, direction, null);
        return weapon.hitAttack;
    }
    public void Attack(GraphExecutor exector, Transform target, Action<Character> onHit)
    {
        weapon.Attack(target);
        weapon.hitAttack += onHit;
    }
    public Action<Character> Attack(GraphExecutor exector, Transform target)
    {
        Attack(exector, target, null);
        return weapon.hitAttack;
    }

    public void Start()
    {
        currentHP = baseStatus.hp + aditionalStatus.hp;
        if(weapon == null)
        {
            weapon = new Weapon.Weapon();
        }
    }
}
[Serializable]
public class Status
{
    public int hp;
    public int attack;
    public int attackCooltime;
    public int criticalChance;
    public int criticalDamage;
}