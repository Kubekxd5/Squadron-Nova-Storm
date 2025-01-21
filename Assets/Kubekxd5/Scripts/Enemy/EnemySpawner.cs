using System.Collections.Generic;
using UnityEngine;

public class SpawnerModule : MonoBehaviour
{
    [Header("Spawn Points")] public List<Transform> spawnPoints = new();

    [Header("Enemy Settings")] public List<GameObject> enemyPrefabs = new();

    public int spawnAmount = 1;
    public float spawnCooldown = 5f;
    public int maxEnemiesToSpawn;

    [Header("Spawner Settings")] public float detectionRadius = 20f;

    public LayerMask playerLayer;
    public bool requirePlayerInRange = true;
    public bool canSpawn = true;

    [Header("Debugging")] public bool showDebugGizmos = true;

    private Transform _player;
    private float _spawnTimer;

    private int _totalSpawnedEnemies;

    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
            Debug.LogWarning("SpawnerModule: No spawn points assigned. Spawning will not work.");

        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
            Debug.LogWarning("SpawnerModule: No enemy prefabs assigned. Spawning will not work.");

        if (GameManager.Instance != null) GameManager.Instance.OnPlayerShipSpawned += FindPlayer;
    }

    private void Update()
    {
        if (!canSpawn)
            return;

        if (requirePlayerInRange && !IsPlayerInRange())
            return;

        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= spawnCooldown)
        {
            SpawnEnemies();
            _spawnTimer = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        if (spawnPoints != null)
            foreach (var point in spawnPoints)
                if (point != null)
                    Gizmos.DrawSphere(point.position, 0.5f);
                else
                    Debug.LogWarning("SpawnerModule: A spawn point reference is null. Please check the inspector.");
    }

    private void FindPlayer()
    {
        var playerObject = GameObject.FindWithTag("PlayerShip");
        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
        else
        {
            _player = null;
            Debug.LogWarning("SpawnerModule: No PlayerShip found in the scene.");
        }
    }

    private bool IsPlayerInRange()
    {
        if (_player == null)
            return false;

        return Vector3.Distance(transform.position, _player.position) <= detectionRadius;
    }

    private void SpawnEnemies()
    {
        if (maxEnemiesToSpawn > 0 && _totalSpawnedEnemies >= maxEnemiesToSpawn)
            return;

        for (var i = 0; i < spawnAmount; i++)
        {
            if (maxEnemiesToSpawn > 0 && _totalSpawnedEnemies >= maxEnemiesToSpawn)
                break;

            SpawnSingleEnemy();
        }
    }

    private void SpawnSingleEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("SpawnerModule: No enemy prefabs assigned. Cannot spawn enemies.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("SpawnerModule: No spawn points assigned. Cannot spawn enemies.");
            return;
        }

        var enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        if (enemyPrefab == null)
        {
            Debug.LogWarning("SpawnerModule: Selected enemy prefab is null. Cannot spawn.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("SpawnerModule: Selected spawn point is null. Cannot spawn.");
            return;
        }

        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        _totalSpawnedEnemies++;
    }

    public void EnableSpawning()
    {
        canSpawn = true;
        Debug.Log("Spawning enabled.");
    }

    public void DisableSpawning()
    {
        canSpawn = false;
        Debug.Log("Spawning disabled.");
    }
}