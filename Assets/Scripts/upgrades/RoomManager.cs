using UnityEngine;

public sealed class RoomManager : MonoBehaviour
{
    [Header("Room")]
    [SerializeField]
    private RoomType roomType =
        RoomType.Combat;

    [Header("References")]
    [SerializeField] private WaveManager waveManager;

    [SerializeField] private RewardSpawner rewardSpawner;

    [SerializeField] private RewardSelectionManager rewardSelectionManager;

    private RoomState currentState =
        RoomState.Inactive;

    public RoomState CurrentState => currentState;

    public RoomType RoomType => roomType;

    private void Start()
    {
        if (rewardSpawner == null)
        {
            Debug.LogError(
                "[RoomManager] RewardSpawner missing.");

            return;
        }

        if (rewardSelectionManager == null)
        {
            Debug.LogError(
                "[RoomManager] RewardSelectionManager missing.");

            return;
        }

        rewardSelectionManager.OnRewardSelectionComplete +=
            HandleRewardSelectionComplete;

        if (roomType == RoomType.Combat)
        {
            if (waveManager == null)
            {
                Debug.LogError(
                    "[RoomManager] WaveManager missing.");

                return;
            }

            waveManager.OnAllWavesCompleted +=
                HandleCombatRoomCleared;

            currentState = RoomState.Combat;

            Debug.Log(
                "[RoomManager] Combat Room Started.");
        }
        else if (roomType == RoomType.Treasure)
        {
            currentState = RoomState.Reward;

            Debug.Log(
                "[RoomManager] Treasure Room Started.");

            // Temporary
            rewardSpawner.SpawnUpgradeRewards();
        }
    }

    private void OnDestroy()
    {
        if (waveManager != null)
        {
            waveManager.OnAllWavesCompleted -=
                HandleCombatRoomCleared;
        }

        if (rewardSelectionManager != null)
        {
            rewardSelectionManager.OnRewardSelectionComplete -=
                HandleRewardSelectionComplete;
        }
    }

    private void HandleCombatRoomCleared()
    {
        currentState = RoomState.Reward;

        Debug.Log(
            "[RoomManager] Combat Complete.");

        GameHUD.Instance?.PlayRoomClearFlash();

        rewardSpawner.SpawnUpgradeRewards();
    }

    private void HandleRewardSelectionComplete()
    {
        currentState = RoomState.Completed;

        if (RunStatsManager.Instance != null)
        {
            RunStatsManager.Instance.AddRoomClear();
        }
        else
        {
            Debug.LogWarning(
                "[RoomManager] RunStatsManager missing. Room clear not recorded.");
        }

        Debug.Log(
            "[RoomManager] Room Completed.");
    }
}
