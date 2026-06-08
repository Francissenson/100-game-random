using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class RewardSelectionManager : MonoBehaviour
{
    private readonly List<RewardInteraction> activeRewards =
        new();

    public event Action OnRewardSelectionComplete;

    public void RegisterReward(
        RewardInteraction reward)
    {
        if (reward == null)
        {
            return;
        }

        if (activeRewards.Contains(reward))
        {
            return;
        }

        activeRewards.Add(reward);

        Debug.Log(
            $"[RewardSelectionManager] Registered Reward. Count: {activeRewards.Count}");
    }

    public void SelectReward(
        RewardInteraction selectedReward)
    {
        Debug.Log(
            "[RewardSelectionManager] Reward Selected");

        foreach (RewardInteraction reward in activeRewards)
        {
            if (reward == null)
            {
                continue;
            }

            Destroy(reward.gameObject);
        }

        activeRewards.Clear();

        Debug.Log(
            "[RewardSelectionManager] Reward Selection Complete");

        OnRewardSelectionComplete?.Invoke();
    }
}