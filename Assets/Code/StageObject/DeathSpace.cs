using UnityEngine;

public class DeathSpace : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDeathable>(out var deathable))
        {
            deathable.Death();
        }
    }
}
