using System.Collections;
using UnityEngine;

public class EnemyRpgBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifespan = 1f;
    public int numberOfBullets = 4; // Patlama sýrasýnda saçýlacak mermi sayýsý
    public GameObject bulletPrefab; // Normal mermilerin prefabý
    public float bulletSpeed = 3f; // Normal mermilerin hýzý
    public float explosionRadius = 2f; // Patlama yarýçapý

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
        Invoke("Explode", lifespan); // Belirli bir süre sonra patla
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Eðer düþman, duvar veya baþka bir nesneye çarparsa patla
        if (hitInfo.CompareTag("Player"))
        {
            hitInfo.gameObject.GetComponent<PlayerMovement>().TakeDamage(1);
            Explode();
        }
    }

    void Explode()
    {
        for (int i = 0; i < numberOfBullets; i++)
        {
            float angle = i * (360f / numberOfBullets);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 direction = rotation * Vector3.up;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = direction * bulletSpeed;
        }

        Destroy(gameObject); // RPG mermisini yok et
    }

}
