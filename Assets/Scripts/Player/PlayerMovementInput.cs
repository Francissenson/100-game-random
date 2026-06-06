using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Reads player movement and dash input.
    /// </summary>
    [RequireComponent(typeof(PlayerCharacter))]
    [RequireComponent(typeof(PlayerDashSystem))]
    public sealed class PlayerMovementInput : MonoBehaviour
    {
        private PlayerCharacter _character;
        private PlayerDashSystem _dashSystem;

        private void Awake()
        {
            _character = GetComponent<PlayerCharacter>();
            _dashSystem = GetComponent<PlayerDashSystem>();
        }

        private void Update()
        {
            ReadMovementInput();
            ReadDashInput();
        }

        private void ReadMovementInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector2 movementDirection =
                new Vector2(horizontal, vertical).normalized;

            _character.SetMovementDirection(movementDirection);
        }

        private void ReadDashInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _dashSystem.TryDash(_character.MovementDirection);
            }
        }
    }
}