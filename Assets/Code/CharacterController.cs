using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private float _attackCooldown = 0f;
    private float speed = 5f;
    public void Move(float direction)
    {
        if(TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.AddForce( new Vector2(direction * speed, rb.linearVelocity.y));
        }
        else
        {
        transform.position += new Vector3(direction * 0.1f, 0, 0);
        }
    }

    public void Attack()
    {
        if (_attackCooldown <= 0)
        {
            Debug.Log("Attack executed!");
            _attackCooldown = 1.0f; // 1秒クールダウン
        }
    }

    void Update()
    {
        if (_attackCooldown > 0)
            _attackCooldown -= Time.deltaTime;
    }
}