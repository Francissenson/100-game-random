using UnityEngine;

/// <summary>
/// Manages player weapon slots and active weapon state.
/// </summary>
public sealed class WeaponManager : MonoBehaviour
{
    [Header("Weapon References")]
    [SerializeField]
    private Transform weaponHolder;

    [Header("Weapon Slots")]
    [SerializeField]
    private Weapon swordWeapon;

    [SerializeField]
    private Weapon pistolWeapon;

    [SerializeField]
    private Weapon lootWeapon;

    private PlayerCombatInput combatInput;
    private Weapon currentWeapon;
    private int currentWeaponSlot = 1;

    public int CurrentWeaponSlot => currentWeaponSlot;

    public Transform WeaponHolder => weaponHolder;

    public Weapon CurrentWeapon => currentWeapon;

    private void Awake()
    {
        combatInput = GetComponent<PlayerCombatInput>();

        if (weaponHolder == null)
        {
            Debug.LogError($"{nameof(WeaponManager)}: Weapon Holder is not assigned.", this);
            return;
        }

        EquipWeapon(1);
    }

    private void Update()
    {
        HandleWeaponSwitching();
        HandleAttackInput();
    }

    private void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeapon(2);
        }
    }

    private void HandleAttackInput()
    {
        if (combatInput == null)
        {
            return;
        }

        if (!combatInput.AttackPressed)
        {
            return;
        }

        currentWeapon?.Attack();
    }

    private void EquipWeapon(int slot)
    {
        if (swordWeapon != null)
        {
            swordWeapon.Unequip();
        }

        if (pistolWeapon != null)
        {
            pistolWeapon.Unequip();
        }

        if (lootWeapon != null)
        {
            lootWeapon.Unequip();
        }

        currentWeapon = null;

        switch (slot)
        {
            case 1:

                if (swordWeapon != null)
                {
                    swordWeapon.Equip();
                    currentWeapon = swordWeapon;
                }

                break;

            case 2:

                if (pistolWeapon != null)
                {
                    pistolWeapon.Equip();
                    currentWeapon = pistolWeapon;
                }

                break;
        }

        currentWeaponSlot = slot;
    }
}   