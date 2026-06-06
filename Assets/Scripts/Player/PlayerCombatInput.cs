using UnityEngine;

/// <summary>
/// Reads player combat input.
/// </summary>
public sealed class PlayerCombatInput : MonoBehaviour
{
    private bool attackPressed;

    /// <summary>
    /// True during the frame attack was pressed.
    /// </summary>
    public bool AttackPressed => attackPressed;

    private void Update()
    {
        attackPressed = Input.GetMouseButtonDown(0);
    }

    private void LateUpdate()
    {
        attackPressed = false;
    }
}