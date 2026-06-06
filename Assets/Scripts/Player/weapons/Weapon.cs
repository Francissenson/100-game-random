using UnityEngine;

/// <summary>
/// Base class for all player weapons.
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Info")]
    [SerializeField]
    private string weaponName = "Weapon";

    [SerializeField]
    private int damage = 10;

    [SerializeField]
    private float attackRate = 1f;

    private float nextAttackTime;

    public string WeaponName => weaponName;
    public int Damage => damage;
    public float AttackRate => attackRate;

    public virtual void Equip()
    {
        gameObject.SetActive(true);
    }

    public virtual void Unequip()
    {
        gameObject.SetActive(false);
    }

    public void Attack()
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        nextAttackTime = Time.time + (1f / attackRate);

        PerformAttack();
    }

    /// <summary>
    /// Weapon-specific attack implementation.
    /// </summary>
    protected abstract void PerformAttack();
}