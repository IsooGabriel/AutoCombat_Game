
using UnityEngine;
namespace Weapon
{
    public class Arrow : Weapon, IDeathable, IDamageable
    {
        [SerializeField]
        private float speed = 8f;

        public void Death()
        {
            Destroy(this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collieder)
        {
            if (collieder.gameObject == user.gameObject || collieder.gameObject == gameObject || !collieder.gameObject.TryGetComponent<IDamageable>(out IDamageable target))
            {
                return;
            }
            target.TakeDamage(damage);
            Death();
        }

        private void Update()
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        void IDamageable.TakeDamage(decimal damage)
        {
            Destroy(gameObject);
        }
    }
}
