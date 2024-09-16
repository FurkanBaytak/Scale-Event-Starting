using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject bulletPrefab; // Fýrlatýlacak mermi prefab'ý
    public int numberOfBullets = 8; // Fýrlatýlacak mermi sayýsý
    public float explosionForce = 10.0f; // Mermilerin fýrlatma gücü

    public void Explode()
    {
        // 360 dereceyi numberOfBullets kadar eþit parçaya bölerek mermileri fýrlat
        float angleStep = 360f / numberOfBullets;
        float angle = 0f;

        for (int i = 0; i < numberOfBullets; i++)
        {
            // Mermiyi oluþtur
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // Merminin yönünü hesapla
            float bulletDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float bulletDirY = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector2 bulletDirection = new Vector2(bulletDirX, bulletDirY).normalized;

            // Merminin Rigidbody2D bileþenini al ve fýrlatma gücünü uygula
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(bulletDirection * explosionForce, ForceMode2D.Impulse);
            }

            // Bir sonraki mermi için açýyý artýr
            angle += angleStep;
        }

        // Patlama sonrasý varili yok et
        Destroy(gameObject);
    }

    // Varile çarptýðýnda patlama iþlemini tetikle
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet")) // Oyuncunun mermisiyle çarpýþma
        {
            Explode();
        }
    }
}
