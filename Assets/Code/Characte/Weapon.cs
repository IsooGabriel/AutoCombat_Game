namespace Weapon
{
    using System;
    using UnityEngine;

    public interface IDestructible
    {
        public decimal TryDestruct(Character destructer, decimal damage);
        public void Destruct();
    }

    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        protected DamageMultiply multiplies = new();
        public const decimal adiCritChanceMult = 2.0m;
        public const decimal criticalMultiplier = 25.0m;
        public decimal damage
        {
            get
            {
                decimal baseDamage = 1m;
                decimal criticalChange = 50m;
                decimal criticalMultipy = 1m;
                if (user != null)
                {
                    float baseAttack = user.baseStatus.attack * multiplies.attack;
                    float addAttack = user.aditionalStatus.attack * multiplies.aditionalAttack;
                    baseDamage = (decimal)((baseAttack + addAttack) * multiplies.finaryAttack);

                    float baseCritCh = user.baseStatus.criticalChance * multiplies.criticalChance;
                    float addCritCh = user.aditionalStatus.criticalChance * multiplies.aditionalCriticalChance;
                    criticalChange = (decimal)((baseCritCh + addCritCh) * multiplies.finaryCriticalChance);

                    float baseMultipy = user.baseStatus.criticalDamage * multiplies.criticalDamage;
                    float addMultipy = user.aditionalStatus.criticalDamage * multiplies.aditionalCriticalDamage;
                    criticalMultipy = (decimal)((baseMultipy + addMultipy)* multiplies.finaryCriticalDamage);
                }

                if ((decimal)UnityEngine.Random.Range(0f, 100f) < criticalChange)
                {

                    baseDamage *= 1 + criticalMultipy;
                }
                return baseDamage * (decimal)damageMultiply * (decimal)multiplies.finaryDamage;
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

        protected const float attackCoottimeMultiply = 0.05f;
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

        [ContextMenu("Reset multipies Only")]
        private void ResetMultipies()
        {
            multiplies = new DamageMultiply();
        }
    }

    [Serializable]
    public class DamageMultiply
    {
        public float finaryDamage = 1f;
        public float attack = 1f;
        public float aditionalAttack = 1f;
        public float finaryAttack = 0.1f;
        public float criticalChance = 1f;
        public float aditionalCriticalChance = 0.2f;
        public float finaryCriticalChance = 1f;
        public float criticalDamage = 1f;
        public float aditionalCriticalDamage = 1f;
        public float finaryCriticalDamage = 0.1f;
    }
}