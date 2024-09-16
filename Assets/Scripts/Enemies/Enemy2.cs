using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : BaseEnemy
{
    public GameObject bulletPrefab; // Mermi prefab'ý
    public float fireRate = 1.0f; // Ateþ etme sýklýðý
    public float stopDistance = 5.0f; // Oyuncuya ne kadar mesafede duracak
    private float nextFireTime;
    private float bulletSpeed = 5.0f;

    protected override void Update()
    {
        if (playerTransform != null)
        {
            // Oyuncuya olan mesafeyi hesapla
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer > stopDistance)
            {
                // Eðer oyuncuya belirlenen mesafeden uzakta ise hareket et
                MoveTowardsPlayer();
            }

            // Ateþ etme davranýþý
            if (distanceToPlayer <= stopDistance && Time.time > nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Fire()
    {
        if (isDead)
        {
            return;
        }
        // Mermiyi oluþtur ve oyuncuya doðru yönlendir
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // Merminin yönünü ve hýzýný ayarlýyoruz
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }

        // Merminin rotasyonunu oyuncuya doðru ayarlýyoruz
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle+90));
    }
}
