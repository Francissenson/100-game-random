using System.Collections.Generic;
using UnityEngine;

public sealed class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private UpgradeDatabase upgradeDatabase;

    private void Awake()
    {
        if (upgradeDatabase == null)
        {
            upgradeDatabase =
                FindFirstObjectByType<UpgradeDatabase>();
        }

        Debug.Log("[UpgradeManager] Initialized.");
    }

    private void Start()
    {
        TryGetPlayerStats();

        if (playerStats != null)
        {
            Debug.Log(
                "[UpgradeManager] PlayerStats connected.");
        }
    }

    private bool TryGetPlayerStats()
    {
        if (playerStats != null)
        {
            return true;
        }

        playerStats =
            FindFirstObjectByType<PlayerStats>();

        return playerStats != null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            ApplyFirstUpgrade();
        }
    }

    private void ApplyFirstUpgrade()
    {
        if (upgradeDatabase == null)
        {
            Debug.LogError(
                "[UpgradeManager] UpgradeDatabase missing.");

            return;
        }

        IReadOnlyList<UpgradeData> upgrades =
            upgradeDatabase.Upgrades;

        if (upgrades.Count == 0)
        {
            Debug.LogWarning(
                "[UpgradeManager] No upgrades available.");

            return;
        }

        ApplyUpgrade(upgrades[0]);
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        if (!TryGetPlayerStats())
        {
            Debug.LogError(
                "[UpgradeManager] PlayerStats not found.");

            return;
        }

        if (upgrade == null)
        {
            Debug.LogError(
                "[UpgradeManager] Upgrade is null.");

            return;
        }

        Debug.Log(
            $"[UpgradeManager] Applying Upgrade: {upgrade.upgradeName}");

        switch (upgrade.upgradeType)
        {
            case UpgradeType.MaxHealth:
                playerStats.AddMaxHealth(upgrade.value);
                break;

            case UpgradeType.MoveSpeed:
                playerStats.AddMoveSpeedMultiplier(
                    upgrade.value);
                break;

            case UpgradeType.Damage:
                playerStats.AddDamageMultiplier(
                    upgrade.value);
                break;

            case UpgradeType.FireRate:
                playerStats.AddFireRateMultiplier(
                    upgrade.value);
                break;

            case UpgradeType.ProjectileCount:
                playerStats.AddProjectileCount(
                    Mathf.RoundToInt(upgrade.value));
                break;

            default:
                Debug.LogWarning(
                    $"[UpgradeManager] Unsupported Upgrade Type: {upgrade.upgradeType}");
                break;
        }
    }
}