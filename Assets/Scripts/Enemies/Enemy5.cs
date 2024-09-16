using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingBulletEnemy : BaseEnemy
{
    public float changeDirectionInterval = 2.0f; // Düþmanýn yön deðiþtirme sýklýðý
    public GameObject bulletPrefab; // Mermi prefab'ý
    public float fireRate = 1.0f; // Ateþ etme sýklýðý
    public float bulletRotationSpeed = 30.0f; // Mermilerin dönerken alacaðý hýz (derece/saniye)
    public bool rotateClockwise = true; // Saat yönünde mi yoksa tersine mi dönecek?
    public float bulletSpeed = 5.0f; // Mermilerin baþlangýç hýzý
    public float stopDuration = 1.0f; // Ateþ ederken durma süresi

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

            // Rastgele yön deðiþtirme
            if (Time.time >= nextChangeDirectionTime)
            {
                isMoving = false;
                stopEndTime = Time.time + stopDuration; // Ne kadar süre duracaðýný belirle
            }
        }
        else
        {
            // Ateþ etme davranýþý
            if (Time.time >= nextFireTime)
            {
                FireRotatingBullets();
                nextFireTime = Time.time + fireRate;
            }

            // Durma süresi sona erdiðinde tekrar hareket et
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
        // Rastgele bir yön seç
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    private void FireRotatingBullets()
    {
        if (isDead)
        {
            return;
        }
        // 4 yöne (yukarý, aþaðý, saða, sola) mermi ateþle
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
