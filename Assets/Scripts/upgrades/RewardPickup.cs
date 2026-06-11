using UnityEngine;

public sealed class RewardPickup : MonoBehaviour
{
    [Header("Reward")]
    [SerializeField] private RewardType rewardType;

    [SerializeField] private UpgradeData upgradeData;

    [SerializeField] private WeaponData weaponData;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer pickupSpriteRenderer;

    public RewardType RewardType => rewardType;
    public UpgradeData UpgradeData => upgradeData;
    public WeaponData WeaponData => weaponData;

    private void Awake()
    {
        if (pickupSpriteRenderer == null)
        {
            pickupSpriteRenderer =
                GetComponent<SpriteRenderer>();
        }

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

    public Sprite GetRewardIcon()
    {
        Sprite pickupSprite =
            GetPickupSprite();

        if (pickupSprite != null)
        {
            return pickupSprite;
        }

        switch (rewardType)
        {
            case RewardType.Upgrade:
                return upgradeData != null
                    ? upgradeData.icon
                    : null;

            case RewardType.Weapon:
                return weaponData != null
                    ? weaponData.icon
                    : null;

            default:
                return null;
        }
    }

    public Sprite GetPickupSprite()
    {
        if (pickupSpriteRenderer == null)
        {
            pickupSpriteRenderer =
                GetComponent<SpriteRenderer>();

            if (pickupSpriteRenderer == null)
            {
                pickupSpriteRenderer =
                    GetComponentInChildren<SpriteRenderer>(true);
            }

            if (pickupSpriteRenderer == null)
            {
                pickupSpriteRenderer =
                    GetComponentInParent<SpriteRenderer>();
            }
        }

        return pickupSpriteRenderer != null
            ? pickupSpriteRenderer.sprite
            : null;
    }

    public void SetUpgradeReward(
        UpgradeData upgradeData)
    {
        rewardType = RewardType.Upgrade;
        this.upgradeData = upgradeData;
        weaponData = null;

        ApplyRewardSprite(
            upgradeData != null ? upgradeData.icon : null,
            upgradeData != null ? upgradeData.upgradeName : "Upgrade");

        Debug.Log(
            $"[RewardPickup] Configured Upgrade Reward: {upgradeData?.upgradeName}");
    }

    public void SetWeaponReward(
        WeaponData weaponData)
    {
        rewardType = RewardType.Weapon;
        this.weaponData = weaponData;
        upgradeData = null;

        ApplyRewardSprite(
            weaponData != null ? weaponData.icon : null,
            weaponData != null ? weaponData.weaponName : "Weapon");

        Debug.Log(
            $"[RewardPickup] Configured Weapon Reward: {weaponData?.weaponName}");
    }

    private void ApplyRewardSprite(
        Sprite rewardIcon,
        string rewardLabel)
    {
        if (pickupSpriteRenderer == null)
        {
            pickupSpriteRenderer =
                GetComponent<SpriteRenderer>();

            if (pickupSpriteRenderer == null)
            {
                pickupSpriteRenderer =
                    GetComponentInChildren<SpriteRenderer>(true);
            }

            if (pickupSpriteRenderer == null)
            {
                pickupSpriteRenderer =
                    GetComponentInParent<SpriteRenderer>();
            }
        }

        if (pickupSpriteRenderer == null)
        {
            Debug.LogError(
                $"[RewardPickup] No SpriteRenderer found for {rewardLabel} pickup.");

            return;
        }

        if (rewardIcon == null)
        {
            Debug.LogWarning(
                $"[RewardPickup] {rewardLabel} has no icon assigned. Keeping prefab sprite.");

            return;
        }

        pickupSpriteRenderer.sprite = rewardIcon;
        Debug.Log(
            $"[RewardPickup] Applied icon '{rewardIcon.name}' to {rewardLabel} pickup.");
    }
}
