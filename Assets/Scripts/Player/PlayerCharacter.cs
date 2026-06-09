using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerCharacter : MonoBehaviour
    {
        [Space(10f)]
        [Tooltip("Movement speed in units per second.")]
        [SerializeField] private float moveSpeed = 6f;

        private Rigidbody2D _rigidbody;
        private PlayerDashSystem _dashSystem;
        private PlayerStats _playerStats;
        private Vector2 _movementDirection;

        public Vector2 MovementDirection => _movementDirection;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _dashSystem = GetComponent<PlayerDashSystem>();
            _playerStats = GetComponent<PlayerStats>();
        }

        private void FixedUpdate()
        {
            if (_dashSystem != null &&
                _dashSystem.IsDashing)
            {
                return;
            }

            ApplyMovement();
        }

        public void SetMovementDirection(
            Vector2 direction)
        {
            _movementDirection = direction;
        }

        private void ApplyMovement()
        {
            float finalMoveSpeed = moveSpeed;

            if (_playerStats != null)
            {
                finalMoveSpeed *=
                    _playerStats.MoveSpeedMultiplier;
            }

            Vector2 targetPosition =
                _rigidbody.position +
                (_movementDirection *
                 finalMoveSpeed *
                 Time.fixedDeltaTime);

            _rigidbody.MovePosition(
                targetPosition);
        }
    }
}