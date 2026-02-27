
using UnityEngine;
namespace Weapon
{
    public class Pusher : DestructibleWeapon, IDeathable, IDamageable
    {
        [SerializeField]
        private float speed = 8f;
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
            Destruct();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody))
            {
                rigidbody.AddForce(rigidBody.linearVelocity * pushForce * Time.deltaTime, ForceMode2D.Impulse);
            }
        }

        private void Update()
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }
}
