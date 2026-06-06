using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Reads mouse aiming input and forwards it to gameplay systems.
    /// </summary>
    [RequireComponent(typeof(WeaponAimController))]
    [RequireComponent(typeof(PlayerSpriteFlipper))]
    public sealed class PlayerAimInput : MonoBehaviour
    {
        private Camera _mainCamera;
        private WeaponAimController _weaponAimController;
        private PlayerSpriteFlipper _spriteFlipper;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _weaponAimController = GetComponent<WeaponAimController>();
            _spriteFlipper = GetComponent<PlayerSpriteFlipper>();
        }

        private void Update()
        {
            UpdateAim();
        }

        private void UpdateAim()
        {
            Vector3 mousePosition =
                _mainCamera.ScreenToWorldPoint(Input.mousePosition);

            mousePosition.z = 0f;

            _weaponAimController.SetAimPosition(mousePosition);
            _spriteFlipper.SetAimPosition(mousePosition);
        }
    }
}