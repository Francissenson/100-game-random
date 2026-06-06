using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Controls player facing direction.
    /// </summary>
    public sealed class PlayerSpriteFlipper : MonoBehaviour
    {
        [Space(10f)]
        [SerializeField] private Transform visualsRoot;

        private Vector3 _aimPosition;

        public void SetAimPosition(Vector3 aimPosition)
        {
            _aimPosition = aimPosition;
        }

        private void LateUpdate()
        {
            UpdateFacing();
        }

        private void UpdateFacing()
        {
            bool facingRight =
                _aimPosition.x >= transform.position.x;

            Vector3 scale = visualsRoot.localScale;

            scale.x = facingRight ? 1f : -1f;

            visualsRoot.localScale = scale;
        }
    }
}