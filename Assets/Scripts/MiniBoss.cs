using System.Collections;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
    private int currentHealth;
    public int maxHealth;
    public Transform player; // Oyuncunun pozisyonu
    public float attackRange = 10f; // Saldýrý menzili
    public float moveSpeed = 2f; // Hareket hýzý
    private float bulletSpeed = 5.0f;

    private float nextFireTime;

    public GameObject projectilePrefab; // Mermi prefabý
    public GameObject rpgBullet; // RPG mermisi prefabý
    public GameObject rotatingBullet; // Dönen mermi prefabý
    public Transform firePoint; // Merminin ateþleneceði nokta

    private bool isPhaseTwo = false; // Faz durumu
    public float fireRate = 0.2f;

    private bool isAttacking = false; // Saldýrýlarýn durmasýný kontrol etmek için

    public Animator animator; // Boss'un Animator bileþeni
    public AudioClip phaseTwoSound; // Faz 2 ses efekti
    public GameObject enemyPrefab; // Spawnlanacak düþman prefabý
    public AudioSource audioSource; // Ses efekti için AudioSource

    void Start()
    {
        currentHealth = maxHealth;
        GameObject player_ = GameObject.FindGameObjectWithTag("Player");
        player = player_.transform;

        // Birinci fazýn saldýrý paternini baþlat
        StartCoroutine(PhaseOneAttackPattern());
    }

    void Update()
    {
        // Faz deðiþimini kontrol et
        if (currentHealth <= maxHealth / 2 && !isPhaseTwo)
        {
            EnterPhaseTwo();
        }

        if (!isAttacking)
        {
            // Oyuncuya yaklaþ veya saldýr
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
        // Oyuncuya doðru hareket et
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    void Attack()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Merminin yönünü ve hýzýný ayarlýyoruz
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }

        // Merminin rotasyonunu oyuncuya doðru ayarlýyoruz
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
    }

    void EnterPhaseTwo()
    {
        isPhaseTwo = true;
        moveSpeed += 1f; // Hýzý arttýr

        // Faz 2 animasyonu tetikle
        if (animator != null)
        {
            animator.SetTrigger("BossUp");
        }

        // Faz 2 ses efekti çal
        if (audioSource != null && phaseTwoSound != null)
        {
            audioSource.PlayOneShot(phaseTwoSound);
        }

        // Düþmanlarý spawnla
        SpawnEnemies();

        // Phase 2 sýrasýnda ayný saldýrý paternine devam et
        StartCoroutine(PhaseTwoAttackPattern());
    }


    void SpawnEnemies()
    {
        // Sahnede düþmanlarýn spawnlanacaðý rastgele x ve y koordinatlarý
        float minX = -10f; // Sahnede spawnlanabilecek minimum x koordinatý
        float maxX = 10f;  // Sahnede spawnlanabilecek maksimum x koordinatý
        float minY = -5f;  // Sahnede spawnlanabilecek minimum y koordinatý
        float maxY = 5f;   // Sahnede spawnlanabilecek maksimum y koordinatý

        for (int i = 0; i < 8; i++)
        {
            // Rastgele bir x ve y pozisyonu seç
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            // Düþmaný bu pozisyonda oluþtur
            Vector2 spawnPosition = new Vector2(randomX, randomY);
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    IEnumerator PhaseOneAttackPattern()
    {
        while (!isPhaseTwo)
        {
            // Bir süre normal saldýrýlar yap
            for (int i = 0; i < 5; i++)
            {
                Attack();
                yield return new WaitForSeconds(1f);
            }

            // Normal saldýrýlarý durdur ve dönen mermiler saç
            isAttacking = true;
            yield return new WaitForSeconds(1f);

            // Özel bir saldýrý: Dönen mermiler saç
            SpawnRotatingBullets();

            // Dönen mermilerden hemen sonra RPG mermileri saç
            yield return new WaitForSeconds(0.5f);
            SpawnRpgBullets();

            // RPG mermileri saçýldýktan sonra bir süre bekle
            yield return new WaitForSeconds(2f);
            isAttacking = false;
        }
    }

    void SpawnRotatingBullets()
    {
        int numberOfBullets = 8; // Etrafa saçýlacak mermi sayýsý
        float angleStep = 360f / numberOfBullets;
        float initialSpeed = 3f; // Mermilerin baþlangýç hýzý
        float rotationSpeed = 45f; // Mermilerin dönüþ hýzý (derece/saniye)

        for (int i = 0; i < numberOfBullets; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Vector2 direction = rotation * Vector2.up; // Merminin hareket edeceði baþlangýç yönü

            // Dönen mermiyi oluþtur ve Initialize metodunu çaðýr
            GameObject bullet = Instantiate(rotatingBullet, transform.position, rotation);
            RotatingBullet rotatingBulletScript = bullet.GetComponent<RotatingBullet>();

            if (rotatingBulletScript != null)
            {
                // Mermiyi baþlat: yön, dönüþ hýzý, saat yönünde dönüþ ve baþlangýç hýzý
                rotatingBulletScript.Initialize(direction, rotationSpeed, i % 2 == 0, initialSpeed);
            }
        }
    }

    void SpawnRpgBullets()
    {
        int numberOfRpgBullets = 4; // Etrafa saçýlacak RPG mermi sayýsý
        float angleStep = 360f / numberOfRpgBullets;

        for (int i = 0; i < numberOfRpgBullets; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Vector2 direction = rotation * Vector2.up;

            // RPG mermisini oluþtur ve yola çýkar
            GameObject rpg = Instantiate(rpgBullet, transform.position, rotation);
            Rigidbody2D rb = rpg.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * 7f; // RPG mermisinin hýzý
            }
        }
    }

    IEnumerator PhaseTwoAttackPattern()
    {
        while (isPhaseTwo)
        {
            // Bir süre normal saldýrýlar yap
            for (int i = 0; i < 5; i++)
            {
                Attack();
                yield return new WaitForSeconds(1f);
            }

            // Normal saldýrýlarý durdur ve dönen mermiler saç
            isAttacking = true;
            yield return new WaitForSeconds(1f);

            // Özel bir saldýrý: Dönen mermiler saç
            SpawnRotatingBullets();

            // Dönen mermilerden hemen sonra RPG mermileri saç
            yield return new WaitForSeconds(0.5f);
            SpawnRpgBullets();

            // RPG mermileri saçýldýktan sonra bir süre bekle
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
