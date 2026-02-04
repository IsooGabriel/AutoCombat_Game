
namespace Weapon
{
    using UnityEngine;
    public class Sword : Weapon
    {
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
            if (hit == user.gameObject || !hit.TryGetComponent<IDamageable>(out IDamageable target))
            {
                return;
            }
            target.TakeDamage(damage);
        }
    }
}