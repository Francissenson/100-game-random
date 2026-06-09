using UnityEngine;

public sealed class TreasureRoomManager : MonoBehaviour
{
    [SerializeField]
    private RewardSpawner rewardSpawner;

    [SerializeField]
    private RewardSelectionManager rewardSelectionManager;

    [SerializeField]
    private RoomExit roomExit;

    private void Start()
    {
        if (rewardSpawner == null)
        {
            Debug.LogError(
                "[TreasureRoomManager] RewardSpawner missing.");

            return;
        }

        rewardSpawner.SpawnTreasureRewards();

        Debug.Log(
            "[TreasureRoomManager] Treasure rewards spawned.");
    }

    private void OnEnable()
    {
        if (rewardSelectionManager != null)
        {
            rewardSelectionManager.OnRewardSelectionComplete +=
                HandleRewardSelected;
        }
    }

    private void OnDisable()
    {
        if (rewardSelectionManager != null)
        {
            rewardSelectionManager.OnRewardSelectionComplete -=
                HandleRewardSelected;
        }
    }

    private void HandleRewardSelected()
    {
        if (roomExit != null)
        {
            roomExit.Unlock();
        }

        Debug.Log(
            "[TreasureRoomManager] Reward selected. Exit unlocked.");
    }
}