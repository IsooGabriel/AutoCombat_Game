using System;
using TMPro;
using UnityEngine;

/// <summary>
/// 死亡可能なオブジェクトのためのインターフェースです。
/// </summary>
public interface IDeathable
{
    /// <summary>
    /// オブジェクトを死亡・消滅させます。
    /// </summary>
    public void Death();
}

/// <summary>
/// ダメージを受けることが可能なオブジェクトのためのインターフェースです。
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// 現在のHPを取得します。
    /// </summary>
    public decimal HPGet { get; }

    /// <summary>
    /// ダメージを与えます。
    /// </summary>
    /// <param name="damage">与えるダメージ量</param>
    public void TakeDamage(decimal damage);
}

public class Character : MonoBehaviour, IDeathable, IDamageable
{
    public Status baseStatus { get; set; } = new() { hp = 10, attack = 10, attackCooltime = 0, criticalChance = 10, criticalDamage = 10 }; // キャラクターの基本ステータス
    public Status aditionalStatus { get; set; } = new() { hp = 0, attack = 0, attackCooltime = 0, criticalChance = 0, criticalDamage = 0 }; // 装備やバフによる追加ステータス
    
    /// <summary>
    /// 基本ステータスと追加ステータスを合算した現在の最終ステータスを取得します。
    /// </summary>
    public Status statsu
    {
        get
        {
            return new Status()
            {
                hp = baseStatus.hp + aditionalStatus.hp,
                attack = baseStatus.attack + aditionalStatus.attack,
                attackCooltime = baseStatus.attackCooltime + aditionalStatus.attackCooltime,
                criticalChance = baseStatus.criticalChance + aditionalStatus.criticalChance,
                criticalDamage = baseStatus.criticalDamage + aditionalStatus.criticalDamage,
            };
        }
    }
    public decimal currentHP = 10; // 現在のヒットポイント
    const decimal speed = 5; // 移動速度の基本定数
    public Weapon.Weapon weapon; // キャラクターが装備している武器
    public TextMeshPro damageText; // ダメージ表示用のテキストプレハブ
    public Action<Character> takeDamage; // ダメージを受けた時に実行されるコールバック
    public Action<Character> onDeath; // 死亡時に実行されるコールバック
    public bool isPlayer = false; // プレイヤーキャラクターかどうか
    public SkinObject[] skins; // キャラクターの外見（スキン）の配列
    [SerializeField]
    private Rigidbody2D rigidBody; // 物理演算用のリジッドボディ
    [SerializeField]
    private float acceleForce = 500f; // 加速（ダッシュ）時の力
    [SerializeField]
    private float acceleCT = 5f; // 加速のクールタイム
    private float acceleTimer = 0; // 加速の残りクールタイム計測用
    [SerializeField]
    private float stopCT = 5f; // 急停止のクールタイム
    private float stopTimer = 0; // 急停止の残りクールタイム計測用

    /// <summary>
    /// キャラクターを死亡させ、シーンから削除します。
    /// </summary>
    public void Death()
    {
        onDeath?.Invoke(this);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// インターフェース経由で現在のHPを取得します。
    /// </summary>
    decimal IDamageable.HPGet => currentHP;

    /// <summary>
    /// 指定されたダメージをキャラクターに与えます。ダメージテキストの生成とHPの減算、死亡判定を行います。
    /// </summary>
    /// <param name="damage">受けるダメージ量</param>
    public void TakeDamage(decimal damage)
    {
        var dam = Instantiate(damageText, transform.position, Quaternion.identity);

        dam.GetComponent<Rigidbody2D>().AddForce((0.5f + Mathf.Clamp(Mathf.Log(1f + (float)damage, 2), 0, Mathf.Log(3, 2))) * Vector2.up * 100);
        dam.fontSize *= Mathf.Log(2f + (float)damage, 2);
        dam.text = (damage*100m).ToString("0");
        dam.color = isPlayer ? Color.red : Color.blue;

        currentHP -= damage;
        takeDamage?.Invoke(this);

        if (currentHP <= 0)
        {
            Death();
        }
    }

    /// <summary>
    /// 指定された方向にキャラクターを移動させます。物理演算が有効な場合は力を加え、無効な場合は座標を直接更新します。
    /// </summary>
    /// <param name="direction">移動する方向ベクトル</param>
    /// <returns>移動後の現在の座標</returns>
    public Vector2 Move(Vector2 direction)
    {
        if (direction.magnitude > 1f)
        {
            direction.Normalize();
        }
        float multiply = (float)speed * 0.035f;
        Debug.Log(Time.deltaTime);
        direction = new Vector2(direction.x * multiply, direction.y * multiply);

        if (rigidBody)
        {
            rigidBody.AddForce(direction * 75, ForceMode2D.Force);
        }
        else
        {
            transform.position += (Vector3)direction;
        }
        return transform.position;
    }

    /// <summary>
    /// 指定された方向に対して武器で攻撃を行います。
    /// </summary>
    /// <param name="exector">実行中のグラフエグゼキューター</param>
    /// <param name="direction">攻撃する方向</param>
    /// <param name="onHit">攻撃がヒットした時のコールバック</param>
    public void Attack(GraphExecutor exector, Vector3 direction, Action<Character> onHit)
    {
        weapon.TryAttack(direction);
        weapon.hitAttack += onHit;
    }

    /// <summary>
    /// 指定された方向に対して武器で攻撃を行い、ヒット時のイベントアクションを返します。
    /// </summary>
    /// <param name="exector">実行中のグラフエグゼキューター</param>
    /// <param name="direction">攻撃する方向</param>
    /// <returns>攻撃ヒット時のアクション</returns>
    public Action<Character> Attack(GraphExecutor exector, Vector2 direction)
    {
        Attack(exector, (Vector3)direction, null);
        return weapon.hitAttack;
    }

    /// <summary>
    /// 指定されたターゲットを武器で攻撃します。
    /// </summary>
    /// <param name="exector">実行中のグラフエグゼキューター</param>
    /// <param name="target">攻撃対象のTransform</param>
    /// <param name="onHit">攻撃がヒットした時のコールバック</param>
    public void Attack(GraphExecutor exector, Transform target, Action<Character> onHit)
    {
        weapon.Attack(target);
        weapon.hitAttack += onHit;
    }

    /// <summary>
    /// 指定されたターゲットを武器で攻撃し、ヒット時のイベントアクションを返します。
    /// </summary>
    /// <param name="exector">実行中のグラフエグゼキューター</param>
    /// <param name="target">攻撃対象のTransform</param>
    /// <returns>攻撃ヒット時のアクション</returns>
    public Action<Character> Attack(GraphExecutor exector, Transform target)
    {
        Attack(exector, target, null);
        return weapon.hitAttack;
    }

    /// <summary>
    /// 指定された方向にキャラクターを急加速させます。クールタイム制限があります。
    /// </summary>
    /// <param name="direction">加速する方向</param>
    public void Accele(Vector2 direction)
    {
        if (acceleTimer > 0)
        {
            return;
        }
        acceleTimer = acceleCT;
        if (direction.magnitude > 1f)
        {
            direction.Normalize();
        }
        rigidBody.AddForce(direction * acceleForce);
    }

    /// <summary>
    /// キャラクターの物理速度を即座にゼロにして停止させます。クールタイム制限があります。
    /// </summary>
    public void Stop()
    {
        if (stopTimer > 0)
        {
            return;
        }
        stopTimer = stopCT;
        rigidBody.linearVelocity = Vector3.zero;
    }

    /// <summary>
    /// 指定されたインデックスに基づいてキャラクターの見た目（スキン）を変更します。
    /// </summary>
    /// <param name="index">スキンのインデックス番号</param>
    public void ChangeSkin(int index)
    {
        if (index < 0 || index >= skins.Length)
        {
            return;
        }
        skins[index].Wear();
    }

    /// <summary>
    /// クールタイムタイマーの更新処理を行います。
    /// </summary>
    public void Update()
    {
        if (acceleTimer > 0)
        {
            acceleTimer -= Time.deltaTime;
        }
        if (stopTimer > 0)
        {
            stopTimer -= Time.deltaTime; // 修正: stopTimerが減るべき箇所
        }
    }

    /// <summary>
    /// キャラクターの初期化処理を行います。
    /// </summary>
    public void Start()
    {
        currentHP = baseStatus.hp + aditionalStatus.hp;
        if (weapon == null)
        {
            weapon = new Weapon.Weapon();
        }
        if (!rigidBody)
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }
    }
}

[Serializable]
public class Status
{
    public int hp; // ヒットポイント
    public int attack; // 攻撃力
    public int attackCooltime; // 攻撃クールタイムの短縮値
    public int criticalChance; // クリティカル率
    public int criticalDamage; // クリティカルダメージ倍率
}

[Serializable]
public class SkinObject
{
    [SerializeField]
    public GameObject[] enable; // 有効化するオブジェクト群
    [SerializeField]
    public GameObject[] disable; // 無効化するオブジェクト群

    /// <summary>
    /// このスキンをキャラクターに適用（装着）します。
    /// </summary>
    public void Wear()
    {
        foreach (var obj in enable)
        {
            obj.SetActive(true);
        }
        foreach (var obj in disable)
        {
            obj.SetActive(false);
        }
    }
}
