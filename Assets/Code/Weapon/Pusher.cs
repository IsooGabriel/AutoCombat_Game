
using UnityEngine;
namespace Weapon
{
    public class Pusher : DestructibleWeapon, IDeathable, IDamageable
    {
        [SerializeField]
        private float pushForce = 5f;
        [SerializeField]
        private Rigidbody2D rigidBody;

        public decimal HPGet => (decimal)hp;

        public void Death()
        {
            Destruct();
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
                if (collided.Contains(collieder.gameObject))
                {
                    return;
                }
                else
                {
                    collided.Add(collieder.gameObject);
                }
                hp -= (float)destructible.TryDestruct(user, (decimal)hp);
                if (hp <= 0)
                {
                    Destruct();
                }
                return;
            }
            if (!collieder.gameObject.TryGetComponent<IDamageable>(out IDamageable target) || target == user as IDamageable)
            {
                return;
            }
            target.TakeDamage(damage);
            if (collieder.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D outrigidbody))
            {
                outrigidbody.AddForce(-rigidBody.linearVelocity.normalized * pushForce, ForceMode2D.Impulse);
            }
            Destruct();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D outrigidbody))
            {
                outrigidbody.AddForce(-rigidBody.linearVelocity.normalized * pushForce, ForceMode2D.Impulse);
            }
        }
    }
}
