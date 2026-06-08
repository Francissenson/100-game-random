using UnityEngine;

public sealed class RewardPickup : MonoBehaviour
{
    [Header("Reward")]
    [SerializeField] private RewardType rewardType;

    [SerializeField] private UpgradeData upgradeData;

    [SerializeField] private WeaponData weaponData;

    public RewardType RewardType => rewardType;
    public UpgradeData UpgradeData => upgradeData;
    public WeaponData WeaponData => weaponData;

    private void Awake()
    {
        Debug.Log(
            $"[RewardPickup] Initialized ({rewardType})");
    }

    public string GetRewardName()
    {
        switch (rewardType)
        {
            case RewardType.Upgrade:
                return upgradeData != null
                    ? upgradeData.upgradeName
                    : "Missing Upgrade";

            case RewardType.Weapon:
                return weaponData != null
                    ? weaponData.weaponName
                    : "Missing Weapon";

            default:
                return "Unknown Reward";
        }
    }

    public string GetRewardDescription()
    {
        switch (rewardType)
        {
            case RewardType.Upgrade:
                return upgradeData != null
                    ? upgradeData.description
                    : "";

            case RewardType.Weapon:
                return weaponData != null
                    ? weaponData.description
                    : "";

            default:
                return "";
        }
    }

    public void SetUpgradeReward(
        UpgradeData upgradeData)
    {
        rewardType = RewardType.Upgrade;
        this.upgradeData = upgradeData;
        weaponData = null;

        Debug.Log(
            $"[RewardPickup] Configured Upgrade Reward: {upgradeData?.upgradeName}");
    }

    public void SetWeaponReward(
        WeaponData weaponData)
    {
        rewardType = RewardType.Weapon;
        this.weaponData = weaponData;
        upgradeData = null;

        Debug.Log(
            $"[RewardPickup] Configured Weapon Reward: {weaponData?.weaponName}");
    }
}