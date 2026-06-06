using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Handles player locomotion.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerCharacter : MonoBehaviour
    {
        [Space(10f)]
        [Tooltip("Movement speed in units per second.")]
        [SerializeField] private float moveSpeed = 6f;

        private Rigidbody2D _rigidbody;
        private PlayerDashSystem _dashSystem;
        private Vector2 _movementDirection;

        public Vector2 MovementDirection => _movementDirection;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _dashSystem = GetComponent<PlayerDashSystem>();
        }

        private void FixedUpdate()
        {
            if (_dashSystem != null && _dashSystem.IsDashing)
            {
                return;
            }

            ApplyMovement();
        }

        /// <summary>
        /// Receives movement direction from the input layer.
        /// </summary>
        public void SetMovementDirection(Vector2 direction)
        {
            _movementDirection = direction;
        }

        private void ApplyMovement()
        {
            Vector2 targetPosition =
                _rigidbody.position +
                (_movementDirection * moveSpeed * Time.fixedDeltaTime);

            _rigidbody.MovePosition(targetPosition);
        }
    }
}