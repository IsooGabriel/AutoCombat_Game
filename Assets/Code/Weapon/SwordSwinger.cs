namespace Weapon
{
    using UnityEngine;
    public class SwordSwinger : Weapon
    {
        [SerializeField]
        private GameObject sword;
        [SerializeField]
        private float swingAngle = 160f;

        public override bool TryAttack(Vector2 direction)
        {
            if (!base.TryAttack(direction))
            {
                return false;
            }

            sword.transform.localPosition = MakeOrigin(originOffset, direction);

            Swing(direction);

            return true;
        }

        async void Swing(Vector2 direction)
        {
            float halfAngle = swingAngle / 2f;
            float duration = Mathf.Clamp((1 / attackSpeed), 0, attackCT);
            float elapsed = 0f;
            Quaternion initialRotation = ArmRotation(direction, -halfAngle);
            Quaternion finalRotation = ArmRotation(direction, halfAngle);
            sword.transform.rotation = initialRotation;

            if (direction.x >= 0)
            {
                (finalRotation, initialRotation) = (initialRotation, finalRotation);
            }
            sword.SetActive(true);

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                sword.transform.rotation = Quaternion.Slerp(initialRotation, finalRotation, t);
                elapsed += Time.deltaTime;
                await System.Threading.Tasks.Task.Yield();
            }

            sword.transform.rotation = initialRotation;
            sword.SetActive(false);
        }

        private void Start()
        {
            if (sword.TryGetComponent<Weapon>(out Weapon swordWeapon))
            {
                swordWeapon.user = user;
                swordWeapon.damageMultiply = damageMultiply;
                swordWeapon.hitAttack = hitAttack;
            }
        }
        private void Update()
        {
            base.Update();
        }
    }
}
