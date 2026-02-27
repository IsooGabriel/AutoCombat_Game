
namespace Weapon
{
    using UnityEngine;
    public class Sword : Weapon
    {
        [SerializeField]
        private float hitCooltime = 0.0f;
        [SerializeField] bool stayHit = false;
        private float hitTimer = 0;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (stayHit) return;
            Hit(collision.gameObject);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!stayHit || hitTimer > 0f) return;
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