namespace Weapon
{
    using System;
    using UnityEngine;

    /// <summary>
    /// 破壊可能なオブジェクトのためのインターフェースです。
    /// </summary>
    public interface IDestructible
    {
        /// <summary>
        /// 指定されたダメージでの破壊を試みます。
        /// </summary>
        /// <param name="destructer">破壊を試みるキャラクター</param>
        /// <param name="damage">与えるダメージ量</param>
        /// <returns>実際に与えたダメージ量</returns>
        public decimal TryDestruct(Character destructer, decimal damage);

        /// <summary>
        /// オブジェクトを破壊します。
        /// </summary>
        public void Destruct();
    }

    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        protected DamageMultiply multiplies = new(); // ダメージ計算に使用する倍率設定
        public const decimal adiCritChanceMult = 2.0m; // 追加クリティカル率の倍率定数
        public const decimal criticalMultiplier = 25.0m; // クリティカル時のダメージ倍率定数

        /// <summary>
        /// 現在のステータスと武器の設定に基づいた最終的なダメージ値を計算して取得します。
        /// </summary>
        public decimal damage
        {
            get
            {
                decimal baseDamage = 1m;
                decimal criticalChange = 50m;
                decimal criticalMultipy = 1m;
                if (user != null)
                {
                    float baseAttack = user.baseStatus.attack * multiplies.attack;
                    float addAttack = user.aditionalStatus.attack * multiplies.aditionalAttack;
                    baseDamage = (decimal)((baseAttack + addAttack) * multiplies.finaryAttack);

                    float baseCritCh = user.baseStatus.criticalChance * multiplies.criticalChance;
                    float addCritCh = user.aditionalStatus.criticalChance * multiplies.aditionalCriticalChance;
                    criticalChange = (decimal)((baseCritCh + addCritCh) * multiplies.finaryCriticalChance);

                    float baseMultipy = user.baseStatus.criticalDamage * multiplies.criticalDamage;
                    float addMultipy = user.aditionalStatus.criticalDamage * multiplies.aditionalCriticalDamage;
                    criticalMultipy = (decimal)((baseMultipy + addMultipy) * multiplies.finaryCriticalDamage);
                }

                if ((decimal)UnityEngine.Random.Range(0f, 100f) < criticalChange)
                {

                    baseDamage *= 1 + criticalMultipy;
                }
                return baseDamage * (decimal)damageMultiply * (decimal)multiplies.finaryDamage;
            }
        }
        protected const float damageMultiply = 0.1f; // ダメージ計算の基礎係数
        public float range; // 武器の攻撃射程
        public float baseCT = 1; // 武器の基礎クールタイム
        [SerializeField]
        private bool usableFirstTime = true; // 開始直後に使用可能かどうか

        /// <summary>
        /// ユーザーのステータスを考慮した現在の攻撃クールタイムを取得します。
        /// </summary>
        public float attackCT
        {
            get
            {
                return baseCT - (baseCT * attackCTMultiply) * (user.baseStatus.attackCooltime + user.aditionalStatus.attackCooltime);
            }
        }
        public const float attackCTMultiply = 1/11; // クールタイム短縮計算の係数
        public Vector2 originOffset; // 攻撃発生位置のオフセット
        public float attackSpeed; // 攻撃の動作速度
        public Character user; // 武器を使用しているキャラクター
        public Action<Character> hitAttack; // 攻撃がヒットした時のコールバック

        public float timer = 0; // 次の攻撃までの残り時間

        /// <summary>
        /// ターゲットにダメージを与え、ヒットイベントを通知します。
        /// </summary>
        /// <param name="target">ダメージを与える対象のキャラクター</param>
        private void SetDamage(Character target)
        {
            target.TakeDamage((int)damage);
            hitAttack?.Invoke(target);
        }

        /// <summary>
        /// 指定された方向への攻撃を試みます。クールタイム中の場合は失敗します。
        /// </summary>
        /// <param name="direction">攻撃を行う方向ベクトル</param>
        /// <returns>攻撃が開始された場合はtrue、クールタイム中の場合はfalse</returns>
        public virtual bool TryAttack(Vector2 direction)
        {
            if (timer > 0)
            {
                return false;
            }
            timer = attackCT;

            return true;
        }

        /// <summary>
        /// 指定されたターゲットに対して攻撃を行います。
        /// </summary>
        /// <param name="target">攻撃対象のTransform</param>
        public virtual void Attack(Transform target)
        {
            TryAttack(target.position - transform.position);
        }

        /// <summary>
        /// オフセットと方向から攻撃の起点となる座標を計算します。
        /// </summary>
        /// <param name="offset">基準となるオフセット</param>
        /// <param name="direction">攻撃の方向</param>
        /// <returns>計算された起点座標</returns>
        public Vector2 MakeOrigin(Vector2 offset, Vector2 direction)
        {
            var result = new Vector2(-direction.normalized.y, direction.normalized.x);
            result = result * offset.y + direction.normalized * offset.x;
            return result;
        }

        /// <summary>
        /// 方向ベクトルから武器や腕の回転クォータニオンを計算します。
        /// </summary>
        /// <param name="direction">向かせたい方向</param>
        /// <returns>計算された回転値</returns>
        protected Quaternion ArmRotation(Vector2 direction)
        {
            return Quaternion.Euler(0, 0, (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
        }

        /// <summary>
        /// 方向ベクトルと角度オフセットから回転クォータニオンを計算します。
        /// </summary>
        /// <param name="direction">向かせたい方向</param>
        /// <param name="angleOffset">角度の補正値</param>
        /// <returns>計算された回転値</returns>
        protected Quaternion ArmRotation(Vector2 direction, float angleOffset)
        {
            return Quaternion.Euler(0, 0, (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + angleOffset);
        }

        /// <summary>
        /// 武器の初期化処理を行います。
        /// </summary>
        protected void Start()
        {
            if (usableFirstTime)
            {
                timer = 0;
            }
            else
            {
                timer = attackCT;
            }
        }

        /// <summary>
        /// クールタイムタイマーの更新を行います。
        /// </summary>
        protected void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
        }

        /// <summary>
        /// インスペクターからダメージ倍率設定をリセットするためのデバッグ用関数です。
        /// </summary>
        [ContextMenu("Reset multipies Only")]
        private void ResetMultipies()
        {
            multiplies = new DamageMultiply();
        }
    }

    [Serializable]
    public class DamageMultiply
    {
        public float finaryDamage = 1f; // 最終ダメージに乗算される倍率
        public float attack = 1f; // 基礎攻撃力に乗算される倍率
        public float aditionalAttack = 1f; // 追加攻撃力に乗算される倍率
        public float finaryAttack = 0.1f; // 攻撃力合算値に乗算される最終倍率
        public float criticalChance = 1f; // 基礎クリティカル率に乗算される倍率
        public float aditionalCriticalChance = 0.2f; // 追加クリティカル率に乗算される倍率
        public float finaryCriticalChance = 1f; // クリティカル率合算値に乗算される最終倍率
        public float criticalDamage = 1f; // 基礎クリティカルダメージに乗算される倍率
        public float aditionalCriticalDamage = 1f; // 追加クリティカルダメージに乗算される倍率
        public float finaryCriticalDamage = 0.1f; // クリティカルダメージ合算値に乗算される最終倍率
    }
}
