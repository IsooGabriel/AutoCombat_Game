namespace Weapon
{
    using UnityEngine;
    public class GammaRayLaser : Weapon
    {
        public override bool TryAttack(Vector2 direction)
        {
            if (!base.TryAttack(direction))
            {
                return false;
            }

            Vector2 origin = (Vector2)transform.position + MakeOrigin(originOffset, direction);
            RaycastHit2D hit = Physics2D.Raycast(origin, (Vector2)direction, range);

            Debug.DrawRay(origin, direction * range, Color.magenta);
            if (hit.collider)
            {
                if (hit.collider.TryGetComponent<IDestructible>(out var destructible))
                {
                    destructible.TryDestruct(user, damage);
                }
                IDamageable target = hit.collider.GetComponent<IDamageable>();
                target?.TakeDamage(damage);
            }
            return true;
        }

        private void Update()
        {
            base.Update();
        }
    }
}
