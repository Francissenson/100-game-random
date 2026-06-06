using UnityEngine;

/// <summary>
/// Basic melee sword weapon.
/// </summary>
public sealed class SwordWeapon : Weapon
{
    [Header("Sword")]
    [SerializeField]
    private SwordHitbox swordHitbox;

    protected override void PerformAttack()
    {
        Debug.Log("Sword Attack");

        if (swordHitbox != null)
        {
            swordHitbox.ActivateHitbox();
        }
    }
}