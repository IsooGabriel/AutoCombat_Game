using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Status baseStatus { get; set; } = new() { hp = 10, attack = 1, attackCooltime = 1, criticalChance = 20, criticalDamage = 40 };
    public Status aditionalStatus { get; set; } = new() { hp = 0, attack = 0, attackCooltime = 0, criticalChance = 0, criticalDamage = 0 };
    const decimal speed = 1;
    public Weapon weapon;
    public Action<Character> takeDamage;
 

    public void TakeDamage(decimal damage)
    {
        baseStatus.hp -= damage;
        takeDamage?.Invoke(this);
    }

    public Vector2 Move(Vector2 direction)
    {
        direction.Normalize();
        float multiply = (float)speed * Time.deltaTime;
        direction = new Vector2(direction.x * multiply, direction.y * multiply);
        transform.position += (Vector3)direction;
        return transform.position;
    }

    public Action<Character> Attack(GraphExecutor exector, Vector2 direction)
    {
        weapon.Attack(exector.myCharacter == this ? exector.enemy : exector.myCharacter);
        return weapon.hitAttack;
    }
}
public class Status
{
    public decimal hp { get; set; }
    public decimal attack { get; set; }
    public decimal attackCooltime { get; set; }
    public decimal criticalChance { get; set; }
    public decimal criticalDamage { get; set; }
}