
namespace Weapon
{
    using UnityEngine;
    public class Arrow:Weapon
    {

        public override bool TryAttack(Vector2 direction)
        {
            if (timer > 0)
            {
                return false;
            }
            timer = attackSpeed;

            return true;
        }
    }
}