using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : BaseEnemy
{
    public GameObject bulletPrefab; // Mermi prefab'�
    public float fireRate = 1.0f; // Ate� etme s�kl���
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
                // E�er oyuncuya belirlenen mesafeden uzakta ise hareket et
                MoveTowardsPlayer();
            }

            // Ate� etme davran���
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
        // Mermiyi olu�tur ve oyuncuya do�ru y�nlendir
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // Merminin y�n�n� ve h�z�n� ayarl�yoruz
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }

        // Merminin rotasyonunu oyuncuya do�ru ayarl�yoruz
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle+90));
    }
}
