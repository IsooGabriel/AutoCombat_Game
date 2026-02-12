namespace Weapon
{
    using System.Collections.Generic;
    using UnityEngine;
    public class DestructibleWeapon : Weapon, IDestructible
    {
        [SerializeField]
        protected float hp = 1;
        protected HashSet<GameObject> collided = new HashSet<GameObject>();
        public virtual decimal TryDestruct(Character destructer, decimal damage)
        {
            if(collided.Contains(destructer.gameObject))
            {
                return 0;
            }
            else
            {
                collided.Add(destructer.gameObject);
            }
            if (destructer == user)
            {
                return 0;
            }
            decimal recoil = (decimal)hp;
            hp -= (float)damage;
            if (hp <= 0)
            {
                Destruct();
            }
            return recoil;
        }
        public virtual void Destruct()
        {
            Destroy(this.gameObject);
        }
    }
}