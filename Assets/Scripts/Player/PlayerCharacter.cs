using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerCharacter : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private string idleState = "Player_Idle";

        [SerializeField] private string runState = "Player_Run";

        [SerializeField] private string deathState = "Player_Death";

        [Space(10f)]
        [Tooltip("Movement speed in units per second.")]
        [SerializeField] private float moveSpeed = 6f;

        private Rigidbody2D _rigidbody;
        private PlayerDashSystem _dashSystem;
        private PlayerStats _playerStats;
        private Animator _animator;
        private Vector2 _movementDirection;
        private bool _isDead;

        public Vector2 MovementDirection => _movementDirection;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _dashSystem = GetComponent<PlayerDashSystem>();
            _playerStats = GetComponent<PlayerStats>();
            _animator = GetComponentInChildren<Animator>(true);

            PlayAnimation(idleState);
        }

        private void FixedUpdate()
        {
            if (AudioManager.IsSceneLoading ||
                TransitionCanvas.IsTransitioning ||
                PauseManager.IsPaused)
            {
                if (_rigidbody != null)
                {
                    _rigidbody.linearVelocity = Vector2.zero;
                }

                return;
            }

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

            if (_isDead)
            {
                return;
            }

            PlayAnimation(
                _movementDirection.sqrMagnitude > 0.001f
                    ? runState
                    : idleState);
        }

        public void PlayDeath()
        {
            _isDead = true;
            PlayAnimation(
                deathState,
                true);
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

        private void PlayAnimation(
            string stateName,
            bool force = false)
        {
            if (_animator == null ||
                string.IsNullOrWhiteSpace(stateName))
            {
                return;
            }

            if (!force &&
                _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                return;
            }

            _animator.Play(
                stateName,
                0,
                0f);
        }
    }
}
