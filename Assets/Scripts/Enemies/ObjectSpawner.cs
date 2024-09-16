using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public List<GameObject> objectsToSpawn;
    public Vector2 spawnAreaMin; 
    public Vector2 spawnAreaMax; 
    public float minSpawnInterval = 1.0f; 
    public float maxSpawnInterval = 5.0f; 

    private void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        while (true)
        {
            // Rastgele bir zaman aral��� se�
            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);

            // Belirlenen s�reyi bekle
            yield return new WaitForSeconds(spawnInterval);

            // Rastgele bir nesne se�
            int randomIndex = Random.Range(0, objectsToSpawn.Count);
            GameObject objectToSpawn = objectsToSpawn[randomIndex];

            // Rastgele bir pozisyon se�
            Vector2 spawnPosition = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            // Nesneyi spawnla
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
