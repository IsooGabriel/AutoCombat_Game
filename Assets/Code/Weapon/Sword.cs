
namespace Weapon
{
    using UnityEngine;
    public class Sword : Weapon
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject == user.gameObject || !collision.gameObject.TryGetComponent<Character>(out Character target))
            {
                return;
            }
            target.TakeDamage((int)damage);
        }
    }
}