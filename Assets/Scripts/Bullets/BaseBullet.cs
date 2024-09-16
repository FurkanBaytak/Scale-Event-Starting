using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        if (collision.CompareTag("Player"))
        {
            player.TakeDamage(damage);
        }
    }
}