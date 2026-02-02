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
        public float baseCT = 1;
        public float attackCT
        {
            get
            {
                return baseCT - attackCoottimeMultiply * (user.baseStatus.attackCooltime + user.aditionalStatus.attackCooltime);
            }
        }

        protected const float attackCoottimeMultiply = 0.1f;
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
            timer = baseCT - attackCoottimeMultiply * (user.baseStatus.attackCooltime + user.aditionalStatus.attackCooltime);

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

        protected Quaternion ArmRotation(Vector2 direction)
        {
            return Quaternion.Euler(0, 0, (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
        }
        protected Quaternion ArmRotation(Vector2 direction, float angleOffset)
        {
            return Quaternion.Euler(0, 0, (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + angleOffset);
        }
        protected void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
        }
    }
}