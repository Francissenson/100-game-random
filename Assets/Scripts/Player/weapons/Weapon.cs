using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public enum FireMode
    {
        SemiAuto,
        Automatic
    }

    [Header("Weapon Settings")]
    [SerializeField] private float attackRate = 2f;

    [SerializeField] private FireMode fireMode = FireMode.SemiAuto;

    private float nextAttackTime;

    protected PlayerStats playerStats;

    public FireMode CurrentFireMode => fireMode;

    protected virtual void Awake()
    {
        playerStats =
            GetComponentInParent<PlayerStats>();

        if (playerStats == null)
        {
            playerStats =
                FindFirstObjectByType<PlayerStats>();
        }

        Debug.Log(
            $"[{GetType().Name}] Initialized");
    }

    public void Attack()
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        float finalAttackRate = attackRate;

        if (playerStats != null)
        {
            finalAttackRate *=
                playerStats.FireRateMultiplier;
        }

        nextAttackTime =
            Time.time + (1f / finalAttackRate);

        PerformAttack();
    }

    protected int GetModifiedDamage(
        int baseDamage)
    {
        if (playerStats == null)
        {
            return baseDamage;
        }

        return Mathf.RoundToInt(
            baseDamage *
            playerStats.DamageMultiplier);
    }

    protected abstract void PerformAttack();
}