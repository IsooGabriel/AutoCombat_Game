
using UnityEngine;
namespace Weapon
{
    public class Arrow : Weapon, IDeathable, IDamageable
    {
        [SerializeField]
        private float speed = 8f;
        [SerializeField] float hp;

        public void Death()
        {
            Destroy(this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collieder)
        {
            if (!collieder.gameObject.TryGetComponent<IDamageable>(out IDamageable target))
            {
                return;
            }
            if (target.HitCheck(user))
            {
                var thp = target.HPGet;
                target.TakeDamage(damage);
                this.TakeDamage(thp);
            }

        }

        private void Update()
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        bool IDamageable.HitCheck(Character user)
        {
            return user != this.user;
        }
        decimal IDamageable.HPGet => (decimal)hp;
        public void TakeDamage(decimal damage)
        {
            hp -= (float)damage;
            if(hp <= 0)Destroy(gameObject);
        }
    }
}
