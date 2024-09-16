using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject bulletPrefab; // F�rlat�lacak mermi prefab'�
    public int numberOfBullets = 8; // F�rlat�lacak mermi say�s�
    public float explosionForce = 10.0f; // Mermilerin f�rlatma g�c�

    public void Explode()
    {
        // 360 dereceyi numberOfBullets kadar e�it par�aya b�lerek mermileri f�rlat
        float angleStep = 360f / numberOfBullets;
        float angle = 0f;

        for (int i = 0; i < numberOfBullets; i++)
        {
            // Mermiyi olu�tur
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // Merminin y�n�n� hesapla
            float bulletDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float bulletDirY = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector2 bulletDirection = new Vector2(bulletDirX, bulletDirY).normalized;

            // Merminin Rigidbody2D bile�enini al ve f�rlatma g�c�n� uygula
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(bulletDirection * explosionForce, ForceMode2D.Impulse);
            }

            // Bir sonraki mermi i�in a��y� art�r
            angle += angleStep;
        }

        // Patlama sonras� varili yok et
        Destroy(gameObject);
    }

    // Varile �arpt���nda patlama i�lemini tetikle
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet")) // Oyuncunun mermisiyle �arp��ma
        {
            Explode();
        }
    }
}
