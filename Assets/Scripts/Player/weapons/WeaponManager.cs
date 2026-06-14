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

    public int CurrentSlot => currentSlot;

    public Transform LootWeaponSlot => lootWeaponSlot;

    public Weapon GetWeaponInSlot(int slot)
    {
        return slot switch
        {
            1 => slot1Weapon,
            2 => slot2Weapon,
            3 => slot3Weapon,
            _ => null
        };
    }

    private void Start()
    {
        EquipSlot(1, false);
    }

    private void Update()
    {
        if (AudioManager.IsSceneLoading ||
            TransitionCanvas.IsTransitioning ||
            PauseManager.IsPaused)
        {
            return;
        }

        HandleKeyboardSwitching();
        HandleMouseWheelSwitching();
    }

    public void Attack()
    {
        if (PauseManager.IsPaused)
        {
            return;
        }

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

        if (!slot3WasActive)
        {
            AudioManager.Instance?.PlayWeaponSwap();
        }

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
        int nextSlot = currentSlot;

        for (int i = 0; i < 3; i++)
        {
            nextSlot++;

            if (nextSlot > 3)
            {
                nextSlot = 1;
            }

            if (HasWeaponInSlot(nextSlot))
            {
                EquipSlot(nextSlot);
                return;
            }
        }
    }

    private void EquipPreviousWeapon()
    {
        int previousSlot = currentSlot;

        for (int i = 0; i < 3; i++)
        {
            previousSlot--;

            if (previousSlot < 1)
            {
                previousSlot = 3;
            }

            if (HasWeaponInSlot(previousSlot))
            {
                EquipSlot(previousSlot);
                return;
            }
        }
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

    private void EquipSlot(
        int slot,
        bool playSwapSfx = true)
    {
        if (!HasWeaponInSlot(slot))
        {
            return;
        }

        int previousSlot = currentSlot;
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

            if (playSwapSfx &&
                previousSlot != currentSlot)
            {
                AudioManager.Instance?.PlayWeaponSwap();
            }
        }
    }
}
