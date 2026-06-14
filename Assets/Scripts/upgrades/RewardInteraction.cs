using UnityEngine;

public sealed class RewardInteraction : MonoBehaviour
{
    [SerializeField] private RewardPickup rewardPickup;

    [SerializeField] private Vector3 hoverTextOffset = new Vector3(0f, 1.2f, 0f);

    [SerializeField] private RewardHoverText hoverCardPrefab;

    private RewardSelectionManager rewardSelectionManager;

    private bool playerInRange;
    private RewardHoverText hoverText;

    private void Awake()
    {
        if (rewardPickup == null)
        {
            rewardPickup =
                GetComponent<RewardPickup>();
        }

        rewardSelectionManager =
            FindFirstObjectByType<RewardSelectionManager>();

        if (rewardSelectionManager != null)
        {
            rewardSelectionManager.RegisterReward(this);
        }
    }

    private void Update()
    {
        if (!playerInRange)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectReward();
        }
    }

    private void SelectReward()
    {
        Debug.Log(
            $"[RewardInteraction] Selected: {rewardPickup.GetRewardName()}");

        switch (rewardPickup.RewardType)
        {
            case RewardType.Upgrade:
                ApplyUpgradeReward();
                break;

            case RewardType.Weapon:
                ApplyWeaponReward();
                break;
        }

        rewardSelectionManager?.SelectReward(this);
    }

    private void ApplyUpgradeReward()
    {
        UpgradeManager upgradeManager =
            FindFirstObjectByType<UpgradeManager>();

        if (upgradeManager == null)
        {
            Debug.LogError(
                "[RewardInteraction] UpgradeManager not found.");

            return;
        }

        upgradeManager.ApplyUpgrade(
            rewardPickup.UpgradeData);
    }

    private void ApplyWeaponReward()
    {
        WeaponData weaponData =
            rewardPickup.WeaponData;

        if (weaponData == null)
        {
            Debug.LogError(
                "[RewardInteraction] WeaponData missing.");

            return;
        }

        if (weaponData.weaponPrefab == null)
        {
            Debug.LogError(
                "[RewardInteraction] Weapon prefab missing.");

            return;
        }

        WeaponManager weaponManager =
            FindFirstObjectByType<WeaponManager>();

        if (weaponManager == null)
        {
            Debug.LogError(
                "[RewardInteraction] WeaponManager not found.");

            return;
        }

        Weapon weaponInstance =
            Instantiate(
                weaponData.weaponPrefab,
                weaponManager.LootWeaponSlot);

        weaponInstance.transform.localPosition =
            Vector3.zero;

        weaponInstance.transform.localRotation =
            Quaternion.identity;

        weaponManager.EquipLootWeapon(
            weaponInstance);

        AudioManager.Instance?.PlayWeaponPickupSfx();

        Debug.Log(
            $"[RewardInteraction] Equipped Weapon: {weaponData.weaponName}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = true;

        Debug.Log(
            $"[RewardInteraction] In Range: {rewardPickup.GetRewardName()}");

        ShowHoverText();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = false;

        Debug.Log(
            $"[RewardInteraction] Left: {rewardPickup.GetRewardName()}");

        HideHoverText();
    }

    private void ShowHoverText()
    {
        if (rewardPickup == null)
        {
            return;
        }

        if (hoverCardPrefab == null)
        {
            Debug.LogError(
                "[RewardInteraction] Hover card prefab not assigned.");

            return;
        }

        if (hoverText == null)
        {
            hoverText =
                Instantiate(
                    hoverCardPrefab);

            hoverText.Bind(
                transform,
                hoverTextOffset);
        }

        Sprite pickupSprite = rewardPickup.GetPickupSprite();

        Debug.Log(
            $"[RewardInteraction] Hover icon: {(pickupSprite != null ? pickupSprite.name : "NULL")}");

        hoverText.SetText(
            rewardPickup.GetRewardName(),
            rewardPickup.GetRewardDescription(),
            pickupSprite);
    }

    private void HideHoverText()
    {
        if (hoverText == null)
        {
            return;
        }

        Destroy(hoverText.gameObject);
        hoverText = null;
    }
}
