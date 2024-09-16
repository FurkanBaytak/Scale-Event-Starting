using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // Spawnlanacak d��man prefab'lerinin listesi
    public float spawnInterval = 0.5f; // D��manlar�n spawnlanma aral���
    public float spawnDistance = 6.0f; // Kameran�n d���na spawnlanacak minimum mesafe
    public Timer timer; // Timer referans�

    private Camera mainCamera;
    private Coroutine spawnCoroutine;

    void Start()
    {
        mainCamera = Camera.main; // Ana kameray� al�yoruz
        spawnCoroutine = StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("Enemy list is empty!");
            return;
        }

        int availableEnemies = Mathf.Clamp((int)(timer.GetCurrentTime() / (timer.totalDuration / 4) * enemyPrefabs.Count), 1, enemyPrefabs.Count);

        // Rastgele bir d��man prefab'i se�iliyor, zamanla daha fazla d��man a��l�yor
        int randomIndex = Random.Range(0, availableEnemies);
        GameObject selectedEnemyPrefab = enemyPrefabs[randomIndex];

        // Kameran�n d���ndaki bir pozisyon belirleniyor
        Vector3 spawnPosition = GetSpawnPositionOutsideCamera();

        // D��man olu�turuluyor
        Instantiate(selectedEnemyPrefab, spawnPosition, Quaternion.identity);
    }

    Vector3 GetSpawnPositionOutsideCamera()
    {
        Vector3 cameraCenter = mainCamera.transform.position;
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        int side = Random.Range(0, 4);
        Vector3 spawnPosition = Vector3.zero;

        switch (side)
        {
            case 0: // Sol
                spawnPosition = new Vector3(cameraCenter.x - cameraWidth / 2 - spawnDistance, Random.Range(cameraCenter.y - cameraHeight / 2, cameraCenter.y + cameraHeight / 2), 0);
                break;
            case 1: // Sa�
                spawnPosition = new Vector3(cameraCenter.x + cameraWidth / 2 + spawnDistance, Random.Range(cameraCenter.y - cameraHeight / 2, cameraCenter.y + cameraHeight / 2), 0);
                break;
            case 2: // �st
                spawnPosition = new Vector3(Random.Range(cameraCenter.x - cameraWidth / 2, cameraCenter.x + cameraWidth / 2), cameraCenter.y + cameraHeight / 2 + spawnDistance, 0);
                break;
            case 3: // Alt
                spawnPosition = new Vector3(Random.Range(cameraCenter.x - cameraWidth / 2, cameraCenter.x + cameraWidth / 2), cameraCenter.y - cameraHeight / 2 - spawnDistance, 0);
                break;
        }

        return spawnPosition;
    }

    public void StopSpawningEnemies()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    public void ResumeSpawningEnemies()
    {
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnEnemies());
        }
    }
}
