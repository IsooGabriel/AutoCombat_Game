
using UnityEngine;

namespace Weapon
{
    public class SubWeaponFire:Weapon
    {
        [SerializeField]
        Weapon[] weapons;
        public override bool TryAttack(Vector2 direction)
        {
            foreach (var wea in weapons)
            {
                wea.user = this.user;
                wea.TryAttack(direction);
            }
            return true;
        }
    }
}
