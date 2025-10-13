using UnityEngine;

public class Entity : MonoBehaviour
{
    private int hp;
    private int attack;
    private int criticalChance;
    private int criticalDamage;
    private int moveSpeed;
    private int attackSpeed;
    private Weapon weapon;
    private NodeManager nodeManager;

    public int Hp { get => hp; set => hp = value; }
    public int Attack { get => attack; set => attack = value; }
    public int CriticalChance { get => criticalChance; set => criticalChance = value; }
    public int CriticalDamage { get => criticalDamage; set => criticalDamage = value; }
    public int MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public int AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    public Weapon Weapon { get => weapon; set => weapon = value; }


    public bool Run()
    {

        return true;
    }

    public bool TakeDamege(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
        return true;
    }

    public bool AttackTarget(Vector2 direction)
    {
        return weapon.Attack(direction);
    }

    protected virtual void Die()
    {
        GameManager.Instance.EntityDie(this);
    }
}