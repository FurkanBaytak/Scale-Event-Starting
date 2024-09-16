using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BaseEnemy : MonoBehaviour
{
    public float speed = 5.0f;
    public int maxHealth = 10;
    public int damage = 1;
    private int currentHealth;
    protected Transform playerTransform;
    public bool isDead = false;

    private bool isKnockedBack = false;

    public AudioClip deathSound;
    private AudioSource audioSource;
    public ParticleSystem deathParticleEffect;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }
        audioSource = GetComponent<AudioSource>();

        if (deathParticleEffect != null)
        {
            deathParticleEffect.Stop();
        }
    }

    protected virtual void Update()
    {
        if (playerTransform != null)
        {
            MoveTowardsPlayer();
        }
    }

    protected virtual void MoveTowardsPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        StartCoroutine(Knockback());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator Knockback()
    {
        if (playerTransform != null)
        {
            isKnockedBack = true;

            Vector3 knockbackDirection = (transform.position - playerTransform.position).normalized;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float knockbackForce = 4.5f;
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                yield return new WaitForSeconds(0.2f);
                rb.AddForce(-knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }

            yield return new WaitForSeconds(0.2f);
            isKnockedBack = false;
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }



        Light2D light = GetComponentInChildren<Light2D>();
        if (light != null)
        {
            light.enabled = false;
        }

        StartCoroutine(PlayDeathEffects());
    }

    private IEnumerator PlayDeathEffects()
    {
        // Ses çalma
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Particle effect çalýþtýrma
        if (deathParticleEffect != null)
        {
            ParticleSystem effect = Instantiate(deathParticleEffect, transform.position, Quaternion.identity);
            effect.Play(); // Particle efektini baþlatýyoruz
            Destroy(effect.gameObject, effect.main.duration); // Particle effect bittiðinde yok et
        }

        // Sesin azalmasý
        if (audioSource != null)
        {
            float startVolume = audioSource.volume;
            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / 0.5f; // 0.3 saniye içinde ses azalýr
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.3f);

        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Die();
            }
        }
    }
}
