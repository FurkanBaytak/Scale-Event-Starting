using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingBulletEnemy : BaseEnemy
{
    public float changeDirectionInterval = 2.0f; // D��man�n y�n de�i�tirme s�kl���
    public GameObject bulletPrefab; // Mermi prefab'�
    public float fireRate = 1.0f; // Ate� etme s�kl���
    public float bulletRotationSpeed = 30.0f; // Mermilerin d�nerken alaca�� h�z (derece/saniye)
    public bool rotateClockwise = true; // Saat y�n�nde mi yoksa tersine mi d�necek?
    public float bulletSpeed = 5.0f; // Mermilerin ba�lang�� h�z�
    public float stopDuration = 1.0f; // Ate� ederken durma s�resi

    private Vector2 moveDirection;
    private float nextChangeDirectionTime;
    private float nextFireTime;
    private bool isMoving = true;
    private float stopEndTime;

    protected override void Start()
    {
        base.Start();
        ChooseNewDirection();
        nextChangeDirectionTime = Time.time + changeDirectionInterval;
    }

    protected override void Update()
    {

        if (isMoving)
        {
            // Hareketi uygula
            transform.position += (Vector3)moveDirection * speed * Time.deltaTime;

            // Rastgele y�n de�i�tirme
            if (Time.time >= nextChangeDirectionTime)
            {
                isMoving = false;
                stopEndTime = Time.time + stopDuration; // Ne kadar s�re duraca��n� belirle
            }
        }
        else
        {
            // Ate� etme davran���
            if (Time.time >= nextFireTime)
            {
                FireRotatingBullets();
                nextFireTime = Time.time + fireRate;
            }

            // Durma s�resi sona erdi�inde tekrar hareket et
            if (Time.time >= stopEndTime)
            {
                isMoving = true;
                ChooseNewDirection();
                nextChangeDirectionTime = Time.time + changeDirectionInterval;
            }
        }
    }

    private void ChooseNewDirection()
    {
        // Rastgele bir y�n se�
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    private void FireRotatingBullets()
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
            RotatingBullet rotatingBullet = bullet.AddComponent<RotatingBullet>();
            rotatingBullet.Initialize(direction, bulletRotationSpeed, rotateClockwise, bulletSpeed);
        }
    }
}
