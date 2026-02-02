
using UnityEngine;
namespace Weapon
{
    public class Arrow : Weapon, IDeathable
    {
        [SerializeField]
        private float speed = 8f;

        public void Death()
        {
            Destroy(this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collieder)
        {
            if (collieder.gameObject == user.gameObject || !collieder.gameObject.TryGetComponent<Character>(out Character target))
            {
                return;
            }
            target.TakeDamage((int)damage);
            Death();
        }

        private void Update()
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }
}
