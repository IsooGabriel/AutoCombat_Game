namespace Weapon
{
using System;
    using UnityEngine;

    public class Weapon : MonoBehaviour
    {
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
        public float attackCT = 1;
        readonly float attackCoottimeMultiply = 0.1f;
        public Vector2 originOffset;
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
            timer = attackCT - attackCoottimeMultiply*(user.baseStatus.attackCooltime + user.aditionalStatus.attackCooltime);
            Vector2 origin = (Vector2)transform.position + MakeOrigin(originOffset, direction);
            RaycastHit2D hit = Physics2D.Raycast(origin, (Vector2)direction, range);

            Debug.DrawRay(origin, direction * range, Color.magenta);
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

        public Vector2 MakeOrigin(Vector2 offset, Vector2 direction)
        {
            var result = new Vector2(-direction.normalized.y, direction.normalized.x);
            result = result * offset.y + direction.normalized * offset.x;
            return result;
        }

        private void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
        }
    }
}