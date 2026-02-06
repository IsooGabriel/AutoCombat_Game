
namespace Weapon
{
    using UnityEngine;
    public class Sword : Weapon
    {
        [SerializeField]
        private float hitCooltime = 0.0f;
        private float timer = 0;
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
            if (timer > 0 || !hit.TryGetComponent<IDamageable>(out IDamageable target))
            {
                return;
            }
            if (target.HitCheck(user))
            {
                timer = hitCooltime;
                target.TakeDamage(damage);
            }
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