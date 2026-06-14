using UnityEngine;

public class PlayerCombatInput : MonoBehaviour
{
    private WeaponManager weaponManager;

    private void Awake()
    {
        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        if (AudioManager.IsSceneLoading ||
            TransitionCanvas.IsTransitioning ||
            PauseManager.IsPaused)
        {
            return;
        }

        if (weaponManager == null)
            return;

        Weapon currentWeapon = weaponManager.CurrentWeapon;

        if (currentWeapon == null)
            return;

        switch (currentWeapon.CurrentFireMode)
        {
            case Weapon.FireMode.SemiAuto:

                if (Input.GetMouseButtonDown(0))
                {
                    weaponManager.Attack();
                }

                break;

            case Weapon.FireMode.Automatic:

                if (Input.GetMouseButton(0))
                {
                    weaponManager.Attack();
                }

                break;
        }
    }
}
