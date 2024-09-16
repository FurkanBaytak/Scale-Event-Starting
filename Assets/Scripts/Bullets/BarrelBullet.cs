using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBullet : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            player.TakeDamage(damage);
        }
            if (collision.tag == "enemy")
            {
                collision.gameObject.GetComponent<BaseEnemy>().TakeDamage(1);
                Destroy(gameObject);
            }
    }
}
