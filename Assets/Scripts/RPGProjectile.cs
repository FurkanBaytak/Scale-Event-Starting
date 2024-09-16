using System.Collections;
using UnityEngine;

public class RPGProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifespan = 1f;
    public int numberOfBullets = 8; // Patlama s�ras�nda sa��lacak mermi say�s�
    public GameObject bulletPrefab; // Normal mermilerin prefab�
    public float bulletSpeed = 15f; // Normal mermilerin h�z�
    public float explosionRadius = 2f; // Patlama yar��ap�

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
        Invoke("Explode", lifespan); // Belirli bir s�re sonra patla
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // E�er d��man, duvar veya ba�ka bir nesneye �arparsa patla
        if (hitInfo.CompareTag("enemy") || hitInfo.CompareTag("Boss") || hitInfo.CompareTag("MiniBoss"))
        {
            if (hitInfo.CompareTag("enemy"))
                hitInfo.gameObject.GetComponent<BaseEnemy>().TakeDamage(5);
            if(hitInfo.CompareTag("MiniBoss"))
                hitInfo.gameObject.GetComponent<MiniBoss>().TakeDamage(5);
            if (hitInfo.CompareTag("Boss"))
            {
                hitInfo.GetComponent<LaserBoss>().TakeDamage(5);
            }
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
