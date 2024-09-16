using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy4 : BaseEnemy
{
    public float changeDirectionInterval = 2.0f; // D��man�n y�n de�i�tirme s�kl���
    public GameObject bulletPrefab; // Mermi prefab'�
    public float fireRate = 1.0f; // Ate� etme s�kl���
    private Vector2 moveDirection;
    private float nextChangeDirectionTime;
    private float nextFireTime;

    protected override void Start()
    {
        base.Start();
        ChooseNewDirection();
        nextChangeDirectionTime = Time.time + changeDirectionInterval;
    }

    protected override void Update()
    {
        base.Update();

        // Rastgele y�n de�i�tirme
        if (Time.time >= nextChangeDirectionTime)
        {
            ChooseNewDirection();
            nextChangeDirectionTime = Time.time + changeDirectionInterval;
        }

        // Hareketi uygula
        transform.position += (Vector3)moveDirection * speed * Time.deltaTime;

        // Ate� etme davran���
        if (Time.time >= nextFireTime)
        {
            FireInAllDirections();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void ChooseNewDirection()
    {
        // Rastgele bir y�n se�
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    private void FireInAllDirections()
    {
        if (isDead)
        {
            return;
        }
        // 4 y�ne (yukar�, a�a��, sa�a, sola) mermi ate�le
        Vector2[] directions = new Vector2[]
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        foreach (Vector2 direction in directions)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * speed;
            }

            // Merminin rotasyonunu ayarl�yoruz
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}
