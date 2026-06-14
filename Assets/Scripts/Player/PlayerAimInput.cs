using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(WeaponAimController))]
    [RequireComponent(typeof(PlayerSpriteFlipper))]
    public sealed class PlayerAimInput : MonoBehaviour
    {
        private Camera _mainCamera;
        private WeaponAimController _weaponAimController;
        private PlayerSpriteFlipper _spriteFlipper;

        private void Awake()
        {
            _weaponAimController = GetComponent<WeaponAimController>();
            _spriteFlipper = GetComponent<PlayerSpriteFlipper>();
        }

        private void Update()
        {
            if (AudioManager.IsSceneLoading ||
                TransitionCanvas.IsTransitioning ||
                PauseManager.IsPaused)
            {
                return;
            }

            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;

                if (_mainCamera == null)
                {
                    return;
                }
            }

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
