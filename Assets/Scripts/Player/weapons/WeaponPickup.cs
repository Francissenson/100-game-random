using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private Weapon weaponPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        WeaponManager weaponManager =
            other.GetComponentInParent<WeaponManager>();

        if (weaponManager == null)
            return;

        if (weaponPrefab == null)
        {
            Debug.LogError("WeaponPickup is missing a weapon prefab reference.");
            return;
        }

        Transform lootSlot = weaponManager.LootWeaponSlot;

        if (lootSlot == null)
        {
            Debug.LogError("Loot Weapon Slot is missing.");
            return;
        }

        Weapon newWeapon =
            Instantiate(
                weaponPrefab,
                lootSlot);

        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

        weaponManager.EquipLootWeapon(newWeapon);

        Destroy(gameObject);
    }
}
