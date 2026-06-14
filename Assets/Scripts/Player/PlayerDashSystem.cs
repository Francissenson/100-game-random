using System.Collections;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Handles player dash behavior.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerDashSystem : MonoBehaviour
    {
        [Space(10f)]
        [SerializeField] private float dashDistance = 3f;

        [SerializeField] private float dashDuration = 0.12f;

        [SerializeField] private float dashCooldown = 0.8f;

        private Rigidbody2D _rigidbody;

        private bool _isDashing;
        private float _nextDashTime;

        public bool IsDashing => _isDashing;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void TryDash(Vector2 direction)
        {
            if (AudioManager.IsSceneLoading ||
                TransitionCanvas.IsTransitioning ||
                PauseManager.IsPaused)
            {
                return;
            }

            if (_isDashing)
            {
                return;
            }

            if (Time.time < _nextDashTime)
            {
                return;
            }

            if (direction == Vector2.zero)
            {
                return;
            }

            StartCoroutine(DashRoutine(direction.normalized));
        }

        private IEnumerator DashRoutine(Vector2 direction)
        {
            _isDashing = true;

            AudioManager.Instance?.PlayDash();

            Vector2 startPosition = _rigidbody.position;
            Vector2 targetPosition = startPosition + direction * dashDistance;

            float elapsedTime = 0f;

            while (elapsedTime < dashDuration)
            {
                elapsedTime += Time.deltaTime;

                float t = elapsedTime / dashDuration;

                // Ease-out curve for a snappier roguelite dash
                t = 1f - Mathf.Pow(1f - t, 3f);

                Vector2 currentPosition =
                    Vector2.Lerp(startPosition, targetPosition, t);

                _rigidbody.MovePosition(currentPosition);

                yield return null;
            }

            _rigidbody.MovePosition(targetPosition);

            _nextDashTime = Time.time + dashCooldown;
            _isDashing = false;
        }
    }
}
