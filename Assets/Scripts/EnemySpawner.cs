using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public enum ZoneType { Plains, Desert, Snow }
    public ZoneType zoneType;

    [Header("Enemy Prefabs (0: Zombie, 1: Mummy, 2: Skeleton)")]
    public List<GameObject> enemyPrefabs; 

    // public float spawnInterval = 3f; // Removed per user request
    public bool isActive = false;
    private bool hasSpawned = false; // Track if we messed up or finished

    void Start()
    {
        if (zoneType == ZoneType.Plains)
        {
            ActivateSpawner();
        }
    }

    public void ActivateSpawner()
    {
        if (hasSpawned) return; // Prevent double activation
        hasSpawned = true;
        isActive = true;
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
             Debug.LogWarning("EnemySpawner: No prefabs assigned!");
             return;
        }

        GameObject prefabToSpawn = null;
        float roll = Random.Range(0f, 1f);

        switch (zoneType)
        {
            case ZoneType.Plains:
                // 50% Zombie (Index 0), 50% Nothing
                if (roll < 0.5f && enemyPrefabs.Count > 0)
                {
                    prefabToSpawn = enemyPrefabs[0];
                }
                break;

            case ZoneType.Desert:
                // 1/3 Zombie (0), 1/3 Mummy (1), 1/3 Nothing
                if (roll < 0.33f && enemyPrefabs.Count > 0)
                {
                    prefabToSpawn = enemyPrefabs[0];
                }
                else if (roll < 0.66f && enemyPrefabs.Count > 1)
                {
                    prefabToSpawn = enemyPrefabs[1];
                }
                break;

            case ZoneType.Snow:
                // 1/3 Zombie (0), 1/3 Skeleton (2), 1/3 Nothing
                if (roll < 0.33f && enemyPrefabs.Count > 0)
                {
                    prefabToSpawn = enemyPrefabs[0];
                }
                else if (roll < 0.66f && enemyPrefabs.Count > 2)
                {
                    prefabToSpawn = enemyPrefabs[2];
                }
                break;
        }

        if (prefabToSpawn != null)
        {
            GameObject newEnemyObj = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            Enemy newEnemyScript = newEnemyObj.GetComponent<Enemy>();
            if (newEnemyScript != null)
            {
                newEnemyScript.originZone = this.zoneType;
                Debug.Log($"Spawner: Spawned enemy. Assigned Origin Zone: {this.zoneType}");
            }
            else
            {
                Debug.LogError("Spawner: Spawned object does not have Enemy script!");
            }
        }
    }
}
