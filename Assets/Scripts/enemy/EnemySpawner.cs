using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject gruntPrefab;
    [SerializeField] private GameObject shooterPrefab;
    [SerializeField] private GameObject tankPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    private readonly List<EnemyHealth> aliveEnemies =
        new List<EnemyHealth>();

    private int nextSpawnPointIndex;

    public int AliveEnemyCount => aliveEnemies.Count;

    public event Action OnEncounterCompleted;

    private void Start()
    {
        Debug.Log("[EnemySpawner] Initialized");
    }

    public void SpawnWave(WaveDefinition wave)
    {
        if (wave == null)
        {
            Debug.LogError("[EnemySpawner] Wave is null.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[EnemySpawner] No spawn points assigned.");
            return;
        }

        nextSpawnPointIndex = 0;

        Debug.Log($"[EnemySpawner] Spawning {wave.waveName}");

        SpawnEnemies(gruntPrefab, wave.gruntCount);
        SpawnEnemies(shooterPrefab, wave.shooterCount);
        SpawnEnemies(tankPrefab, wave.tankCount);
    }

    private void SpawnEnemies(
        GameObject prefab,
        int count)
    {
        if (prefab == null || count <= 0)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint =
                spawnPoints[
                    nextSpawnPointIndex % spawnPoints.Length];

            nextSpawnPointIndex++;

            GameObject enemy =
                Instantiate(
                    prefab,
                    spawnPoint.position,
                    Quaternion.identity);

            EnemyHealth enemyHealth =
                enemy.GetComponent<EnemyHealth>();

            if (enemyHealth == null)
            {
                Debug.LogError(
                    $"{enemy.name} missing EnemyHealth.");

                continue;
            }

            enemyHealth.OnDeath += HandleEnemyDeath;

            aliveEnemies.Add(enemyHealth);

            Debug.Log(
                $"Spawned {enemy.name} | Alive: {aliveEnemies.Count}");
        }
    }

    private void HandleEnemyDeath(
        EnemyHealth enemyHealth)
    {
        enemyHealth.OnDeath -= HandleEnemyDeath;

        aliveEnemies.Remove(enemyHealth);

        if (RunStatsManager.Instance != null)
        {
            RunStatsManager.Instance.AddKill();
        }
        else
        {
            Debug.LogWarning(
                "[EnemySpawner] RunStatsManager missing. Kill not recorded.");
        }

        Debug.Log(
            $"Enemy Removed | Alive: {aliveEnemies.Count}");

        if (aliveEnemies.Count == 0)
        {
            Debug.Log("[EnemySpawner] Encounter completed.");

            OnEncounterCompleted?.Invoke();
        }
    }
}
