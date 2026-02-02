
namespace Weapon
{
    using UnityEngine;
    public class BowAndArrow : Weapon
    {
        [SerializeField]
        private GameObject arrowPrefab;
        [SerializeField]
        private GameObject bow;

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
            Debug.Log(direction);
            GameObject arrow = Instantiate
                (
                    arrowPrefab,
                    transform.position + (Vector3)MakeOrigin(originOffset, direction),
                    bow.transform.rotation * Quaternion.Euler( 0, 0, -bowAngle),
                    transform.root.parent
                );

            Arrow arrowComponent;
            if (!arrow.TryGetComponent<Arrow>(out arrowComponent)) 
            {
                arrowComponent = arrow.AddComponent<Arrow>();
            }
            SettingArrow(arrowComponent);

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

        private void SettingArrow(Arrow arrowComponent)
        {
            arrowComponent.user = user;
            arrowComponent.damageMultiply = damageMultiply;
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