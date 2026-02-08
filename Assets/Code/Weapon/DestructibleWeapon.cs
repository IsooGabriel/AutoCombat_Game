namespace Weapon
{
    using UnityEngine;
    public class DestructibleWeapon : Weapon, IDestructible
    {
        [SerializeField]
        protected float hp = 1;
        [SerializeField]
        protected float recoilMultiply = 1;
        public virtual decimal TryDestruct(Character destructer, decimal damage)
        {
            if (destructer == user)
            {
                return 0;
            }
            decimal recoil = (decimal)hp * (decimal)recoilMultiply;
            hp -= (float)damage;
            if (hp <= 0)
            {
                Destruct();
            }
            return recoil;
        }
        public virtual void Destruct()
        {
            Destroy(gameObject);
        }
    }
}