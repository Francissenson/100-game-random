using System.Collections.Generic;
using UnityEngine;

public sealed class RewardSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UpgradeSelection upgradeSelection;

    [SerializeField] private WeaponDatabase weaponDatabase;

    [SerializeField] private GameObject rewardPickupPrefab;

    [Header("Spawn Positions")]
    [SerializeField] private Transform[] spawnPoints;

    public void SpawnUpgradeRewards()
    {
        if (upgradeSelection == null)
        {
            Debug.LogError(
                "[RewardSpawner] UpgradeSelection missing.");

            return;
        }

        if (rewardPickupPrefab == null)
        {
            Debug.LogError(
                "[RewardSpawner] Reward Prefab missing.");

            return;
        }

        List<UpgradeData> rewards =
            upgradeSelection.GenerateChoices(3);

        for (int i = 0;
             i < rewards.Count && i < spawnPoints.Length;
             i++)
        {
            GameObject rewardObject =
                Instantiate(
                    rewardPickupPrefab,
                    spawnPoints[i].position,
                    Quaternion.identity);

            RewardPickup rewardPickup =
                rewardObject.GetComponent<RewardPickup>();

            if (rewardPickup == null)
            {
                Debug.LogError(
                    "[RewardSpawner] RewardPickup missing on prefab.");

                continue;
            }

            rewardPickup.SetUpgradeReward(
                rewards[i]);
        }

        Debug.Log(
            $"[RewardSpawner] Spawned {rewards.Count} upgrade rewards.");
    }

    public void SpawnTreasureRewards()
    {
        if (upgradeSelection == null)
        {
            Debug.LogError(
                "[RewardSpawner] UpgradeSelection missing.");

            return;
        }

        if (weaponDatabase == null)
        {
            Debug.LogError(
                "[RewardSpawner] WeaponDatabase missing.");

            return;
        }

        if (rewardPickupPrefab == null)
        {
            Debug.LogError(
                "[RewardSpawner] Reward Prefab missing.");

            return;
        }

        if (spawnPoints.Length < 3)
        {
            Debug.LogError(
                "[RewardSpawner] Treasure rewards require 3 spawn points.");

            return;
        }

        WeaponData weaponReward =
            weaponDatabase.GetRandomWeapon();

        List<UpgradeData> upgrades =
            upgradeSelection.GenerateChoices(2);

        SpawnWeaponReward(
            spawnPoints[0],
            weaponReward);

        for (int i = 0;
             i < upgrades.Count && i < 2;
             i++)
        {
            SpawnUpgradeReward(
                spawnPoints[i + 1],
                upgrades[i]);
        }

        Debug.Log(
            "[RewardSpawner] Spawned Treasure Rewards.");
    }

    private void SpawnWeaponReward(
        Transform spawnPoint,
        WeaponData weaponData)
    {
        GameObject rewardObject =
            Instantiate(
                rewardPickupPrefab,
                spawnPoint.position,
                Quaternion.identity);

        RewardPickup rewardPickup =
            rewardObject.GetComponent<RewardPickup>();

        if (rewardPickup == null)
        {
            Debug.LogError(
                "[RewardSpawner] RewardPickup missing on prefab.");

            return;
        }

        rewardPickup.SetWeaponReward(
            weaponData);
    }

    private void SpawnUpgradeReward(
        Transform spawnPoint,
        UpgradeData upgradeData)
    {
        GameObject rewardObject =
            Instantiate(
                rewardPickupPrefab,
                spawnPoint.position,
                Quaternion.identity);

        RewardPickup rewardPickup =
            rewardObject.GetComponent<RewardPickup>();

        if (rewardPickup == null)
        {
            Debug.LogError(
                "[RewardSpawner] RewardPickup missing on prefab.");

            return;
        }

        rewardPickup.SetUpgradeReward(
            upgradeData);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnUpgradeRewards();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnTreasureRewards();
        }
    }
}