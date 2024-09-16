using System.Collections;
using UnityEngine;

public class LaserBoss : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Transform laserSpawnPoint; // Lazerlerin ��kaca�� tek nokta
    public GameObject laserPrefab; // Lazer prefab�
    public GameObject laserWallPrefab; // Lazer duvar� prefab�
    public GameObject enemyPrefab; // Spawnlanacak d��man prefab�
    public GameObject healingObjectPrefab; // �yile�me i�in spawnlanacak objeler
    public float timeBetweenAttacks = 7f; // Sald�r�lar aras�ndaki s�re
    public float laserActiveDuration = 25f; // Lazerin aktif kalaca�� s�re
    public float attackDelay = 2f; // Boss do�duktan sonra sald�r�ya ba�lama gecikmesi
    public float healingRate = 5f; // Boss'un iyile�me h�z�
    private int healingObjectCount = 4; // �yile�me objelerinin say�s�

    private bool isPhaseTwo = false;
    private bool isHealing = false;

    public Animator bossAnimator; // Boss'un Animator bile�eni
    public AudioSource audioSource; // Ses efekti i�in AudioSource
    public AudioClip phaseTwoSound; // Faz 2'ye ge�erken �al�nacak ses efekti

    public GameObject Player;

    private GameObject[] healingObjects; // �yile�me objeleri

    void Start()
    {
        currentHealth = maxHealth;
        Player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(StartAttackAfterDelay(attackDelay));
    }

    void Update()
    {
        if (currentHealth <= maxHealth / 2 && !isPhaseTwo)
        {
            EnterPhaseTwo();
        }

        if (isHealing)
        {
            HealBoss();
        }
    }

    private void EnterPhaseTwo()
    {
        isPhaseTwo = true;
        BossManager.Instance.UpdateSliderForPhaseTwo();
        // Faz 2 animasyonunu tetikle
        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger("LaserBossUp");
        }

        // Faz 2 ses efekti �al
        if (audioSource != null && phaseTwoSound != null)
        {
            audioSource.PlayOneShot(phaseTwoSound);
        }

        StartCoroutine(Phase2AttackPattern());
    }

    IEnumerator StartAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(Phase1AttackPattern());
    }

    IEnumerator Phase1AttackPattern()
    {
        while (currentHealth > 0 && !isPhaseTwo)
        {
            // Phase 1'deki sald�r� paternleri s�ral� olacak
            yield return RotateLasers();
            yield return RotateLasersLeft();
            yield return MultipleLaserShots();
            yield return SpawnEnemiesAndLaserAttack();
            yield return SpawnHealingObjects();

            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    IEnumerator Phase2AttackPattern()
    {
        while (currentHealth > 0)
        {
            // Phase 2'de sald�r�lar rastgele uygulanacak
            int randomAttack = Random.Range(0, 5);
            switch (randomAttack)
            {
                case 0:
                    yield return RotateLasers();
                    break;
                case 1:
                    yield return RotateLasersLeft();
                    break;
                case 2:
                    yield return MultipleLaserShots();
                    break;
                case 3:
                    yield return SpawnEnemiesAndLaserAttack();
                    break;
                case 4:
                    yield return SpawnHealingObjects();
                    break;
                
            }

            yield return new WaitForSeconds(timeBetweenAttacks/2);
        }
    }

    IEnumerator RotateLasers()
    {
        int numberOfLasers = 6;
        float angleStep = 360f / numberOfLasers;

        // 6 y�ne lazer ��kar
        GameObject[] lasers = new GameObject[numberOfLasers];
        for (int i = 0; i < numberOfLasers; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Vector3 spawnPosition = laserSpawnPoint.position;

            lasers[i] = Instantiate(laserPrefab, spawnPosition, rotation);
            LaserController laserController = lasers[i].GetComponent<LaserController>();
            if (laserController != null)
            {
                float laserDuration = 18f; // Lazer s�resi
                laserController.ActivateLaser(laserDuration);
            }
        }

        // Lazerleri tam bir tur d�nd�r
        float rotateSpeed = 20f; // Derece/saniye
        for (float rotatedAngle = 0f; rotatedAngle < 60f; rotatedAngle += rotateSpeed * Time.deltaTime)
        {
            foreach (var laser in lasers)
            {
                if (laser != null)
                {
                    laser.transform.RotateAround(laserSpawnPoint.position, Vector3.forward, rotateSpeed * Time.deltaTime);
                }
            }
            yield return null;
        }
    }

    IEnumerator RotateLasersLeft()
    {
        int numberOfLasers = 6;
        float angleStep = 360f / numberOfLasers;

        // 6 y�ne lazer ��kar
        GameObject[] lasers = new GameObject[numberOfLasers];
        for (int i = 0; i < numberOfLasers; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Vector3 spawnPosition = laserSpawnPoint.position;

            lasers[i] = Instantiate(laserPrefab, spawnPosition, rotation);
            LaserController laserController = lasers[i].GetComponent<LaserController>();
            if (laserController != null)
            {
                float laserDuration = 18f; // Lazer s�resi
                laserController.ActivateLaser(laserDuration);
            }
        }

        // Lazerleri tam bir tur ters y�nde d�nd�r
        float rotateSpeed = 30f; // Derece/saniye
        for (float rotatedAngle = 0f; rotatedAngle < 60f; rotatedAngle += rotateSpeed * Time.deltaTime)
        {
            foreach (var laser in lasers)
            {
                if (laser != null)
                {
                    laser.transform.RotateAround(laserSpawnPoint.position, Vector3.forward, -rotateSpeed * Time.deltaTime);
                }
            }
            yield return null;
        }
    }

    IEnumerator MultipleLaserShots()
    {
        int numberOfLasers = 18;
        float angleStep = 360f / numberOfLasers;

        for (int i = 0; i < numberOfLasers; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Vector3 spawnPosition = laserSpawnPoint.position;

            GameObject laser = Instantiate(laserPrefab, spawnPosition, rotation);
            LaserController laserController = laser.GetComponent<LaserController>();
            if (laserController != null)
            {
                laserController.ActivateLaser(laserActiveDuration);
            }
        }

        yield return new WaitForSeconds(laserActiveDuration);

        for (int i = 0; i < numberOfLasers; i++)
        {
            float angle = i * angleStep + 30f; // 30 derece kayd�rarak
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Vector3 spawnPosition = laserSpawnPoint.position;

            GameObject laser = Instantiate(laserPrefab, spawnPosition, rotation);
            LaserController laserController = laser.GetComponent<LaserController>();
            if (laserController != null)
            {
                laserController.ActivateLaser(laserActiveDuration);
            }
        }

        yield return new WaitForSeconds(laserActiveDuration);
    }

    IEnumerator SpawnEnemiesAndLaserAttack()
    {
        int numberOfEnemies = 6;
        GameObject[] enemies = new GameObject[numberOfEnemies];

        // D��manlar� spawnla
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector2 spawnPosition = new Vector2(Random.Range(-5f, 5f), Random.Range(-3f, 3f));
            enemies[i] = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }

        // D��manlar hayattayken lazerlerle sald�r
        while (true)
        {
            bool allEnemiesDead = true;
            foreach (var enemy in enemies)
            {
                if (enemy != null)
                {
                    allEnemiesDead = false;
                }
            }

            FireLaserAtTarget(Player.transform);

            if (allEnemiesDead)
            {
                break;
            }

            yield return new WaitForSeconds(2f);
        }
    }

    void FireLaserAtTarget(Transform target)
    {
        Vector3 direction = (target.position - laserSpawnPoint.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, -direction);

        GameObject laser = Instantiate(laserPrefab, laserSpawnPoint.position, rotation);
        LaserController laserController = laser.GetComponent<LaserController>();
        if (laserController != null)
        {
            laserController.ActivateLaser(2f, target); // Duration 2 saniye
        }
    }

    IEnumerator SpawnHealingObjects()
    {
        healingObjects = new GameObject[healingObjectCount];
        isHealing = true;

        for (int i = 0; i < healingObjectCount; i++)
        {
            Vector2 spawnPosition = new Vector2(Random.Range(-5f, 5f), Random.Range(-3f, 3f));
            healingObjects[i] = Instantiate(healingObjectPrefab, spawnPosition, Quaternion.identity);
        }

        // Objeler var oldu�u s�rece iyile�me devam edecek
        while (isHealing)
        {
            CheckHealingObjects();
            yield return new WaitForSeconds(1f);
        }
    }

    void CheckHealingObjects()
    {
        isHealing = false;
        foreach (var obj in healingObjects)
        {
            if (obj != null)
            {
                isHealing = true;
                break;
            }
        }
    }

    void HealBoss()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += (int)(healingRate * Time.deltaTime);
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        BossManager.Instance.LazerbossHealthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Boss'un yok edilmesi
        Destroy(gameObject, 2f); // �l�m animasyonu i�in biraz gecikme
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
