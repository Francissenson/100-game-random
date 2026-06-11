using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Handles weapon aiming rotation.
    /// </summary>
    public sealed class WeaponAimController : MonoBehaviour
    {
        [Space(10f)]
        [SerializeField] private Transform weaponPivot;

        private Vector3 _aimPosition;

        private void LateUpdate()
        {
            RotateWeapon();
        }

        public void SetAimPosition(Vector3 aimPosition)
        {
            _aimPosition = aimPosition;
        }

        private void RotateWeapon()
        {
            Vector2 direction =
                _aimPosition - weaponPivot.position;

            bool aimingLeft =
                _aimPosition.x < weaponPivot.position.x;

            Vector3 localScale = weaponPivot.localScale;

            localScale.y =
                Mathf.Abs(localScale.y) * (aimingLeft ? -1f : 1f);

            weaponPivot.localScale = localScale;

            float angle =
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            weaponPivot.rotation =
                Quaternion.Euler(0f, 0f, angle);
        }
    }
}
