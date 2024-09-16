using System.Collections;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
    private int currentHealth;
    public int maxHealth;
    public Transform player; // Oyuncunun pozisyonu
    public float attackRange = 10f; // Sald�r� menzili
    public float moveSpeed = 2f; // Hareket h�z�
    private float bulletSpeed = 5.0f;

    private float nextFireTime;

    public GameObject projectilePrefab; // Mermi prefab�
    public GameObject rpgBullet; // RPG mermisi prefab�
    public GameObject rotatingBullet; // D�nen mermi prefab�
    public Transform firePoint; // Merminin ate�lenece�i nokta

    private bool isPhaseTwo = false; // Faz durumu
    public float fireRate = 0.2f;

    private bool isAttacking = false; // Sald�r�lar�n durmas�n� kontrol etmek i�in

    public Animator animator; // Boss'un Animator bile�eni
    public AudioClip phaseTwoSound; // Faz 2 ses efekti
    public GameObject enemyPrefab; // Spawnlanacak d��man prefab�
    public AudioSource audioSource; // Ses efekti i�in AudioSource

    void Start()
    {
        currentHealth = maxHealth;
        GameObject player_ = GameObject.FindGameObjectWithTag("Player");
        player = player_.transform;

        // Birinci faz�n sald�r� paternini ba�lat
        StartCoroutine(PhaseOneAttackPattern());
    }

    void Update()
    {
        // Faz de�i�imini kontrol et
        if (currentHealth <= maxHealth / 2 && !isPhaseTwo)
        {
            EnterPhaseTwo();
        }

        if (!isAttacking)
        {
            // Oyuncuya yakla� veya sald�r
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange && Time.time > nextFireTime)
            {
                Attack();
                nextFireTime = Time.time + fireRate;
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
    }

    void MoveTowardsPlayer()
    {
        // Oyuncuya do�ru hareket et
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    void Attack()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Merminin y�n�n� ve h�z�n� ayarl�yoruz
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }

        // Merminin rotasyonunu oyuncuya do�ru ayarl�yoruz
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
    }

    void EnterPhaseTwo()
    {
        isPhaseTwo = true;
        moveSpeed += 1f; // H�z� artt�r

        // Faz 2 animasyonu tetikle
        if (animator != null)
        {
            animator.SetTrigger("BossUp");
        }

        // Faz 2 ses efekti �al
        if (audioSource != null && phaseTwoSound != null)
        {
            audioSource.PlayOneShot(phaseTwoSound);
        }

        // D��manlar� spawnla
        SpawnEnemies();

        // Phase 2 s�ras�nda ayn� sald�r� paternine devam et
        StartCoroutine(PhaseTwoAttackPattern());
    }


    void SpawnEnemies()
    {
        // Sahnede d��manlar�n spawnlanaca�� rastgele x ve y koordinatlar�
        float minX = -10f; // Sahnede spawnlanabilecek minimum x koordinat�
        float maxX = 10f;  // Sahnede spawnlanabilecek maksimum x koordinat�
        float minY = -5f;  // Sahnede spawnlanabilecek minimum y koordinat�
        float maxY = 5f;   // Sahnede spawnlanabilecek maksimum y koordinat�

        for (int i = 0; i < 8; i++)
        {
            // Rastgele bir x ve y pozisyonu se�
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            // D��man� bu pozisyonda olu�tur
            Vector2 spawnPosition = new Vector2(randomX, randomY);
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    IEnumerator PhaseOneAttackPattern()
    {
        while (!isPhaseTwo)
        {
            // Bir s�re normal sald�r�lar yap
            for (int i = 0; i < 5; i++)
            {
                Attack();
                yield return new WaitForSeconds(1f);
            }

            // Normal sald�r�lar� durdur ve d�nen mermiler sa�
            isAttacking = true;
            yield return new WaitForSeconds(1f);

            // �zel bir sald�r�: D�nen mermiler sa�
            SpawnRotatingBullets();

            // D�nen mermilerden hemen sonra RPG mermileri sa�
            yield return new WaitForSeconds(0.5f);
            SpawnRpgBullets();

            // RPG mermileri sa��ld�ktan sonra bir s�re bekle
            yield return new WaitForSeconds(2f);
            isAttacking = false;
        }
    }

    void SpawnRotatingBullets()
    {
        int numberOfBullets = 8; // Etrafa sa��lacak mermi say�s�
        float angleStep = 360f / numberOfBullets;
        float initialSpeed = 3f; // Mermilerin ba�lang�� h�z�
        float rotationSpeed = 45f; // Mermilerin d�n�� h�z� (derece/saniye)

        for (int i = 0; i < numberOfBullets; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Vector2 direction = rotation * Vector2.up; // Merminin hareket edece�i ba�lang�� y�n�

            // D�nen mermiyi olu�tur ve Initialize metodunu �a��r
            GameObject bullet = Instantiate(rotatingBullet, transform.position, rotation);
            RotatingBullet rotatingBulletScript = bullet.GetComponent<RotatingBullet>();

            if (rotatingBulletScript != null)
            {
                // Mermiyi ba�lat: y�n, d�n�� h�z�, saat y�n�nde d�n�� ve ba�lang�� h�z�
                rotatingBulletScript.Initialize(direction, rotationSpeed, i % 2 == 0, initialSpeed);
            }
        }
    }

    void SpawnRpgBullets()
    {
        int numberOfRpgBullets = 4; // Etrafa sa��lacak RPG mermi say�s�
        float angleStep = 360f / numberOfRpgBullets;

        for (int i = 0; i < numberOfRpgBullets; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Vector2 direction = rotation * Vector2.up;

            // RPG mermisini olu�tur ve yola ��kar
            GameObject rpg = Instantiate(rpgBullet, transform.position, rotation);
            Rigidbody2D rb = rpg.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * 7f; // RPG mermisinin h�z�
            }
        }
    }

    IEnumerator PhaseTwoAttackPattern()
    {
        while (isPhaseTwo)
        {
            // Bir s�re normal sald�r�lar yap
            for (int i = 0; i < 5; i++)
            {
                Attack();
                yield return new WaitForSeconds(1f);
            }

            // Normal sald�r�lar� durdur ve d�nen mermiler sa�
            isAttacking = true;
            yield return new WaitForSeconds(1f);

            // �zel bir sald�r�: D�nen mermiler sa�
            SpawnRotatingBullets();

            // D�nen mermilerden hemen sonra RPG mermileri sa�
            yield return new WaitForSeconds(0.5f);
            SpawnRpgBullets();

            // RPG mermileri sa��ld�ktan sonra bir s�re bekle
            yield return new WaitForSeconds(2f);
            isAttacking = false;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        MiniBossManager.Instance.bossHealthSlider.value = currentHealth;

        if (currentHealth <= maxHealth / 2)
        {
            MiniBossManager.Instance.UpdateSliderForPhaseTwo();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Boss defeated!");
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
        }
    }
}
