using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private float _attackCooldown = 0f;

    public void Move(float direction)
    {
        transform.position += new Vector3(direction * 0.1f, 0, 0);
        Debug.Log($"Moved: {direction}");
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