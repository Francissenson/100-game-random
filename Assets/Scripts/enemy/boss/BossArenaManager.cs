using System.Collections;
using UnityEngine;

public sealed class BossArenaManager : MonoBehaviour
{
    [Header("Wave")]
    [SerializeField]
    private WaveManager waveManager;

    [Header("Boss")]
    [SerializeField]
    private EnemyHealth bossPrefab;

    [SerializeField]
    private Transform bossSpawnPoint;

    [Header("Room")]
    [SerializeField]
    private RoomExit roomExit;

    [SerializeField]
    private float bossSpawnDelay = 3f;

    private EnemyHealth currentBoss;

    private bool bossSpawned;

    private void OnEnable()
    {
        if (waveManager != null)
        {
            waveManager.OnAllWavesCompleted +=
                HandleAllWavesCompleted;
        }
    }

    private void OnDisable()
    {
        if (waveManager != null)
        {
            waveManager.OnAllWavesCompleted -=
                HandleAllWavesCompleted;
        }

        if (currentBoss != null)
        {
            currentBoss.OnDeath -=
                HandleBossDeath;
        }
    }

    private void HandleAllWavesCompleted()
    {
        if (bossSpawned)
        {
            return;
        }

        StartCoroutine(
            SpawnBossAfterDelay());
    }

    private IEnumerator SpawnBossAfterDelay()
    {
        bossSpawned = true;

        Debug.Log(
            "[BossArenaManager] All enemies defeated.");

        yield return new WaitForSeconds(
            bossSpawnDelay);

        SpawnBoss();
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogError(
                "[BossArenaManager] Boss Prefab missing.");

            return;
        }

        if (bossSpawnPoint == null)
        {
            Debug.LogError(
                "[BossArenaManager] Boss Spawn Point missing.");

            return;
        }

        currentBoss =
            Instantiate(
                bossPrefab,
                bossSpawnPoint.position,
                Quaternion.identity);

        currentBoss.OnDeath +=
            HandleBossDeath;

        Debug.Log(
            "[BossArenaManager] Boss Spawned.");
    }

    private void HandleBossDeath(
        EnemyHealth boss)
    {
        Debug.Log(
            "[BossArenaManager] Boss Defeated!");

        if (roomExit != null)
        {
            roomExit.Unlock();
        }
    }
}