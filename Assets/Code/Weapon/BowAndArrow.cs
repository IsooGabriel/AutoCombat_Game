
namespace Weapon
{
    using UnityEngine;
    public class BowAndArrow : Weapon
    {
        [SerializeField]
        private GameObject arrowPrefab;
        [SerializeField]
        private GameObject bow;
        [SerializeField] float arrowSpeed=10;
        private float bowSpeed = 100f;
        private readonly float bowHideDelay = 0.2f;
        private float bowHideTimer = 0f;
        private float bowAngle = -90f;
        public override bool TryAttack(Vector2 direction)
        {
            BowAnimation(direction);

            if (!base.TryAttack(direction))
            {
                return false;
            }
            GameObject arrow = Instantiate
                (
                    arrowPrefab,
                    transform.position + (Vector3)MakeOrigin(originOffset, direction),
                    bow.transform.rotation * Quaternion.Euler( 0, 0, -bowAngle),
                    transform.root.parent
                );

            Weapon arrowComponent;
            if (arrow.TryGetComponent<Weapon>(out arrowComponent)) 
            {
                SettingArrow(arrowComponent);
            }
            Rigidbody2D rigid;
            if (arrow.TryGetComponent<Rigidbody2D>(out rigid))
            {
                rigid.AddForce(direction.normalized * arrowSpeed);
            }

            return true;
        }

        private void BowAnimation(Vector2 direction)
        {
            bowHideTimer = 0f;
            bow.SetActive(true);
            Vector2 origin = (Vector2)transform.position + MakeOrigin(originOffset, direction);
            bow.transform.position = Vector2.MoveTowards(bow.transform.position, origin, bowSpeed * Time.deltaTime);
            bow.transform.rotation = ArmRotation(direction, bowAngle);
        }

        private void SettingArrow(Weapon arrowComponent)
        {
            arrowComponent.user = user;
            arrowComponent.hitAttack = hitAttack;
        }

        private void Update()
        {
            base.Update();

            if (!bow.activeSelf)
            {
                return;
            }
            bowHideTimer += Time.deltaTime;
            if (bowHideTimer >= bowHideDelay)
            {
                bow.SetActive(false);
                bowHideTimer = 0f;
            }
        }
    }
}