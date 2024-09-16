using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingBullet : BaseBullet
{
    private Vector2 initialDirection;
    private float rotationSpeed;
    private bool rotateClockwise;
    private Rigidbody2D rb;



    public void Initialize(Vector2 initialDirection, float rotationSpeed, bool rotateClockwise, float initialSpeed)
    {
        this.initialDirection = initialDirection;
        this.rotationSpeed = rotationSpeed;
        this.rotateClockwise = rotateClockwise;
        rb = GetComponent<Rigidbody2D>();

        // Baþlangýç hýzýný belirle
        rb.velocity = initialDirection * initialSpeed;
    }

    private void Update()
    {
        // Mermiyi döndür
        float angle = rotationSpeed * Time.deltaTime;
        if (!rotateClockwise)
        {
            angle = -angle;
        }

        Vector2 newDirection = RotateVector(initialDirection, angle);
        if(rb != null) { rb.velocity = newDirection * rb.velocity.magnitude; }
        initialDirection = newDirection;

        // Merminin rotasyonunu görsel olarak ayarlýyoruz
        float visualAngle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, visualAngle));
    }

    private Vector2 RotateVector(Vector2 vector, float angle)
    {
        float radianAngle = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radianAngle);
        float sin = Mathf.Sin(radianAngle);

        float newX = vector.x * cos - vector.y * sin;
        float newY = vector.x * sin + vector.y * cos;

        return new Vector2(newX, newY).normalized;
    }
}