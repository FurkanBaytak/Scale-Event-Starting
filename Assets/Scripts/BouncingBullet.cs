using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBullet : MonoBehaviour
{
    public float bulletSpeed;
    public float bulletLifeSpan;
    public int maxBounces;
    public float bounceRadius;

    private Rigidbody2D rb;
    private int bounceCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, bulletLifeSpan);
    }

    public void Initialize(float speed, float lifespan, int maxBounces, float radius)
    {
        this.bulletSpeed = speed;
        this.bulletLifeSpan = lifespan;
        this.maxBounces = maxBounces;
        this.bounceRadius = radius;
    }

    void Update()
    {
        rb.velocity = transform.up * bulletSpeed;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("enemy") || hitInfo.CompareTag("Boss") || hitInfo.CompareTag("MiniBoss"))
        {
            if (hitInfo.CompareTag("MiniBoss"))
            {
                hitInfo.gameObject.GetComponent<MiniBoss>().TakeDamage(1);
            }
            else if (hitInfo.CompareTag("Boss"))
            {
                hitInfo.gameObject.GetComponent<LaserBoss>().TakeDamage(1);
            }
            else if (hitInfo.CompareTag("enemy")){
                hitInfo.gameObject.GetComponent<BaseEnemy>().TakeDamage(1);
            }
            bounceCount++;
            if (bounceCount >= maxBounces)
            {
                Destroy(gameObject);
                return;
            }

            GameObject nearestEnemy = FindNearestEnemy(hitInfo.transform.position);
            if (nearestEnemy != null)
            {
                Vector2 direction = (nearestEnemy.transform.position - transform.position).normalized;
                transform.up = direction;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    GameObject FindNearestEnemy(Vector2 currentPosition)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(currentPosition, bounceRadius);
        float nearestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (Collider2D hit in hits)
        {
            if ((hit.CompareTag("enemy") || hit.CompareTag("Boss") || hit.CompareTag("MiniBoss")) && hit.transform.position != (Vector3)currentPosition)
            {
                float distance = Vector2.Distance(currentPosition, hit.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = hit.gameObject;
                }
            }
        }

        return nearestEnemy;
    }
}
