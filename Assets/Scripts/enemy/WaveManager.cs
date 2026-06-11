using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Waves")]
    [SerializeField]
    private List<WaveDefinition> waves =
        new List<WaveDefinition>();

    [Header("Timing")]
    [SerializeField] private float waveStartDelay = 3f;

    private int currentWaveIndex = -1;

    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCompleted;
    public event Action<WaveReward> OnRewardGranted;
    public event Action OnAllWavesCompleted;

    private void Start()
    {
        if (enemySpawner == null)
        {
            Debug.LogError(
                "[WaveManager] EnemySpawner reference missing.");

            return;
        }

        if (waves.Count == 0)
        {
            Debug.LogError(
                "[WaveManager] No waves configured.");

            return;
        }

        enemySpawner.OnEncounterCompleted +=
            HandleWaveCompleted;

        Debug.Log(
            "[WaveManager] Initialized.");

        StartCoroutine(
            StartNextWaveRoutine());
    }

    private void OnDestroy()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnEncounterCompleted -=
                HandleWaveCompleted;
        }
    }

    private IEnumerator StartNextWaveRoutine()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log(
                "[WaveManager] All waves completed.");

            OnAllWavesCompleted?.Invoke();

            yield break;
        }

        WaveDefinition wave =
            waves[currentWaveIndex];

        Debug.Log(
            $"[WaveManager] Next wave starting in {waveStartDelay} seconds.");

        yield return new WaitForSeconds(
            waveStartDelay);

        Debug.Log(
            $"[WaveManager] Starting {wave.waveName}");

        OnWaveStarted?.Invoke(
            currentWaveIndex + 1);

        enemySpawner.SpawnWave(
            wave);
    }

    private void HandleWaveCompleted()
    {
        WaveDefinition completedWave =
            waves[currentWaveIndex];

        Debug.Log(
            $"[WaveManager] Wave {currentWaveIndex + 1} completed.");

        OnWaveCompleted?.Invoke(
            currentWaveIndex + 1);

        if (completedWave.reward != null)
        {
            Debug.Log(
                $"[WaveManager] Reward Granted: {completedWave.reward.goldReward} Gold");

            if (RunStatsManager.Instance != null)
            {
                RunStatsManager.Instance.AddGold(
                    completedWave.reward.goldReward);
            }
            else
            {
                Debug.LogWarning(
                    "[WaveManager] RunStatsManager missing. Gold not recorded.");
            }

            OnRewardGranted?.Invoke(
                completedWave.reward);
        }

        StartCoroutine(
            StartNextWaveRoutine());
    }
}
