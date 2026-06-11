using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class CombatRoomManager : MonoBehaviour
{
    [Serializable]
    private sealed class WaveSection
    {
        public string name;
        public WaveManager waveManager;
        public SectionUnlocker unlocker;
    }

    [Header("References")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private RewardSpawner rewardSpawner;
    [SerializeField] private RewardSelectionManager rewardSelectionManager;
    [SerializeField] private RoomExit roomExit;

    [Header("Wave Sections")]
    [SerializeField]
    private List<WaveSection> waveSections =
        new();

    private readonly List<WaveSection> activeSections =
        new();

    private readonly List<Action> sectionHandlers =
        new();

    private readonly HashSet<int> completedSections =
        new();

    private void OnEnable()
    {
        CacheActiveSections();
        SubscribeWaveSections();

        if (activeSections.Count == 0 &&
            waveManager != null)
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
        UnsubscribeWaveSections();

        if (activeSections.Count == 0 &&
            waveManager != null)
        {
            waveManager.OnAllWavesCompleted -= HandleWavesCompleted;
        }

        if (rewardSelectionManager != null)
        {
            rewardSelectionManager.OnRewardSelectionComplete -= HandleRewardSelectionComplete;
        }
    }

    private void CacheActiveSections()
    {
        activeSections.Clear();
        completedSections.Clear();

        foreach (WaveSection section in waveSections)
        {
            if (section == null ||
                section.waveManager == null)
            {
                continue;
            }

            activeSections.Add(section);
        }
    }

    private void SubscribeWaveSections()
    {
        sectionHandlers.Clear();

        for (int i = 0; i < activeSections.Count; i++)
        {
            int sectionIndex = i;

            Action handler =
                () => HandleWaveSectionCompleted(sectionIndex);

            activeSections[i].waveManager.OnAllWavesCompleted += handler;
            sectionHandlers.Add(handler);
        }
    }

    private void UnsubscribeWaveSections()
    {
        for (int i = 0;
             i < activeSections.Count &&
             i < sectionHandlers.Count;
             i++)
        {
            activeSections[i].waveManager.OnAllWavesCompleted -=
                sectionHandlers[i];
        }

        sectionHandlers.Clear();
    }

    private void HandleWaveSectionCompleted(
        int sectionIndex)
    {
        if (!completedSections.Add(sectionIndex))
        {
            return;
        }

        Debug.Log(
            $"[CombatRoomManager] Wave Section {sectionIndex + 1} Completed");

        bool finalSection =
            completedSections.Count >= activeSections.Count;

        if (!finalSection)
        {
            SectionUnlocker unlocker =
                activeSections[sectionIndex].unlocker;

            if (unlocker == null)
            {
                Debug.LogWarning(
                    $"[CombatRoomManager] Section {sectionIndex + 1} has no unlocker.");

                return;
            }

            unlocker.UnlockSection();
            return;
        }

        HandleWavesCompleted();
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

        if (RunStatsManager.Instance != null)
        {
            RunStatsManager.Instance.AddRoomClear();
        }
        else
        {
            Debug.LogWarning(
                "[CombatRoomManager] RunStatsManager missing. Room clear not recorded.");
        }

        if (roomExit != null)
        {
            roomExit.Unlock();
        }
    }
}
