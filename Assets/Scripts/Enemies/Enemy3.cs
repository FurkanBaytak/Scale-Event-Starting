using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3 : BaseEnemy
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
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        GameObject bulletCenter = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Rigidbody2D rbCenter = bulletCenter.GetComponent<Rigidbody2D>();
        if (rbCenter != null)
        {
            rbCenter.velocity = direction * bulletSpeed;
        }

        float angleCenter = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bulletCenter.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleCenter + 90));

        // Sol mermiyi 30 derece sola yönlendir
        Vector3 directionLeft = Quaternion.Euler(0, 0, 30) * direction;
        GameObject bulletLeft = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Rigidbody2D rbLeft = bulletLeft.GetComponent<Rigidbody2D>();
        if (rbLeft != null)
        {
            rbLeft.velocity = directionLeft * bulletSpeed;
        }

        float angleLeft = Mathf.Atan2(directionLeft.y, directionLeft.x) * Mathf.Rad2Deg;
        bulletLeft.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleLeft + 90));

        Vector3 directionRight = Quaternion.Euler(0, 0, -30) * direction;
        GameObject bulletRight = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Rigidbody2D rbRight = bulletRight.GetComponent<Rigidbody2D>();
        if (rbRight != null)
        {
            rbRight.velocity = directionRight * bulletSpeed;
        }

        float angleRight = Mathf.Atan2(directionRight.y, directionRight.x) * Mathf.Rad2Deg;
        bulletRight.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleRight + 90));
    }
}
