
namespace Weapon
{
    using UnityEngine;
    public class Sword : Weapon
    {
        [SerializeField]
        private float hitCooltime = 0.0f;
        private float hitTimer = 0;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Hit(collision.gameObject);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Hit(collision.gameObject);
        }
        void Hit(GameObject hit)
        {
            if (hit.TryGetComponent<DestructibleWeapon>(out DestructibleWeapon destructible))
            {
                destructible.Destruct();
                return;
            }
            if (timer > 0 || !hit.TryGetComponent<IDamageable>(out IDamageable target))
            {
                return;
            }
            hitTimer = hitCooltime;
            target.TakeDamage(damage);

        }

        private void Update()
        {
            if (hitTimer > 0)
            {
                hitTimer -= Time.deltaTime;
            }
        }
    }
}