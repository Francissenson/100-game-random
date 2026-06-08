using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Slots")]
    [SerializeField] private Weapon slot1Weapon;
    [SerializeField] private Weapon slot2Weapon;

    [SerializeField] private Transform lootWeaponSlot;

    private Weapon slot3Weapon;

    private Weapon currentWeapon;

    private int currentSlot = 1;

    public Weapon CurrentWeapon => currentWeapon;

    public Transform LootWeaponSlot => lootWeaponSlot;

    private void Start()
    {
        EquipSlot(1);
    }

    private void Update()
    {
        HandleKeyboardSwitching();
        HandleMouseWheelSwitching();
    }

    public void Attack()
    {
        currentWeapon?.Attack();
    }

    public void EquipLootWeapon(Weapon newWeapon)
    {
        bool slot3WasActive = currentSlot == 3;

        if (slot3Weapon != null)
        {
            Destroy(slot3Weapon.gameObject);
        }

        slot3Weapon = newWeapon;

        slot3Weapon.gameObject.SetActive(false);

        if (slot3WasActive)
        {
            EquipSlot(3);
        }
    }

    private void HandleKeyboardSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipSlot(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipSlot(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (slot3Weapon != null)
            {
                EquipSlot(3);
            }
        }
    }

    private void HandleMouseWheelSwitching()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll > 0f)
        {
            EquipNextWeapon();
        }
        else if (scroll < 0f)
        {
            EquipPreviousWeapon();
        }
    }

    private void EquipNextWeapon()
    {
        int startSlot = currentSlot;

        do
        {
            currentSlot++;

            if (currentSlot > 3)
            {
                currentSlot = 1;
            }

            if (HasWeaponInSlot(currentSlot))
            {
                EquipSlot(currentSlot);
                return;
            }

        } while (currentSlot != startSlot);
    }

    private void EquipPreviousWeapon()
    {
        int startSlot = currentSlot;

        do
        {
            currentSlot--;

            if (currentSlot < 1)
            {
                currentSlot = 3;
            }

            if (HasWeaponInSlot(currentSlot))
            {
                EquipSlot(currentSlot);
                return;
            }

        } while (currentSlot != startSlot);
    }

    private bool HasWeaponInSlot(int slot)
    {
        return slot switch
        {
            1 => slot1Weapon != null,
            2 => slot2Weapon != null,
            3 => slot3Weapon != null,
            _ => false
        };
    }

    private void EquipSlot(int slot)
    {
        if (slot1Weapon != null)
            slot1Weapon.gameObject.SetActive(false);

        if (slot2Weapon != null)
            slot2Weapon.gameObject.SetActive(false);

        if (slot3Weapon != null)
            slot3Weapon.gameObject.SetActive(false);

        switch (slot)
        {
            case 1:
                currentWeapon = slot1Weapon;
                break;

            case 2:
                currentWeapon = slot2Weapon;
                break;

            case 3:
                currentWeapon = slot3Weapon;
                break;
        }

        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(true);
            currentSlot = slot;
        }
    }
}