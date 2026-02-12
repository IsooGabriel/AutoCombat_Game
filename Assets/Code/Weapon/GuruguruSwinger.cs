using DG.Tweening;
using UnityEngine;
namespace Weapon
{
    public class GuruguruSwinger : Weapon
    {
        [SerializeField]
        private GameObject sword;
        [SerializeField] float activeTime;
        [SerializeField]
        float guruguruSpeed;
        private float animationSpeed = 10f;

        private float duration
        {
            get
            {
                return activeTime;
            }
        }

        public override bool TryAttack(Vector2 direction)
        {
            Vector2 origin = (Vector2)user.transform.position + MakeOrigin(originOffset, direction);
            sword.transform.position = origin;
            sword.transform.localPosition = Vector3.zero;

            if (!base.TryAttack(direction))
            {
                return false;
            }


            Swing(direction);

            return true;
        }

        async void Swing(Vector2 direction)
        {
            float elapsed = 0f;
            sword.transform.rotation = ArmRotation(direction);
            sword.SetActive(true);

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                sword.transform.rotation = ArmRotation(direction,guruguruSpeed * elapsed);
                elapsed += Time.deltaTime;
                await System.Threading.Tasks.Task.Yield();
            }
            sword.SetActive(false);
        }

        private void Start()
        {
            if (sword.TryGetComponent<Weapon>(out Weapon swordWeapon))
            {
                swordWeapon.user = user;
                swordWeapon.hitAttack = hitAttack;
            }
        }
        private void Update()
        {
            base.Update();
        }
    }
}
