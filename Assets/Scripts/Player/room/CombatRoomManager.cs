using UnityEngine;

public sealed class CombatRoomManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private RewardSpawner rewardSpawner;
    [SerializeField] private RewardSelectionManager rewardSelectionManager;
    [SerializeField] private RoomExit roomExit;

    private void OnEnable()
    {
        if (waveManager != null)
        {
            waveManager.OnAllWavesCompleted += HandleWavesCompleted;
        }

        if (rewardSelectionManager != null)
        {
            rewardSelectionManager.OnRewardSelectionComplete += HandleRewardSelectionComplete;
        }
    }

    private void OnDisable()
    {
        if (waveManager != null)
        {
            waveManager.OnAllWavesCompleted -= HandleWavesCompleted;
        }

        if (rewardSelectionManager != null)
        {
            rewardSelectionManager.OnRewardSelectionComplete -= HandleRewardSelectionComplete;
        }
    }

    private void HandleWavesCompleted()
    {
        Debug.Log("[CombatRoomManager] All Waves Completed");

        if (rewardSpawner != null)
        {
            rewardSpawner.SpawnUpgradeRewards();
        }
    }

    private void HandleRewardSelectionComplete()
    {
        Debug.Log("[CombatRoomManager] Reward Selected");

        if (roomExit != null)
        {
            roomExit.Unlock();
        }
    }
}