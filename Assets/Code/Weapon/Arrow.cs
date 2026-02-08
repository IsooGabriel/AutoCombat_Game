
using UnityEngine;
namespace Weapon
{
    public class Arrow : DestructibleWeapon, IDeathable, IDamageable
    {
        [SerializeField]
        private float speed = 8f;
        public decimal HPGet => (decimal)hp;

        public void Death()
        {
            Destroy(this.gameObject);
        }
        public void TakeDamage(decimal damage)
        {
            hp -= (float)damage;
            if (hp <= 0)
            {
                Death();
            }
        }

        private void OnTriggerEnter2D(Collider2D collieder)
        {
            if (collieder.gameObject.TryGetComponent<IDestructible>(out IDestructible destructible))
            {
                decimal recoil = destructible.TryDestruct(user, (decimal)hp);
                if (hp - (float)recoil < 0)
                {
                    Destruct();
                }
                return;
            }
            if (!collieder.gameObject.TryGetComponent<IDamageable>(out IDamageable target))
            {
                return;
            }
            target.TakeDamage(damage);
            Destruct();
        }

        private void Update()
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }
}
