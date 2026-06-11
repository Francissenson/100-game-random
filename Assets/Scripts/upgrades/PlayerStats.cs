using System;
using UnityEngine;

public sealed class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealthBonus;

    [Header("Movement")]
    [SerializeField] private float moveSpeedMultiplier = 1f;

    [Header("Combat")]
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private float fireRateMultiplier = 1f;

    [Header("Projectiles")]
    [SerializeField] private int bonusProjectiles;

    public float MaxHealthBonus => maxHealthBonus;
    public float MoveSpeedMultiplier => moveSpeedMultiplier;
    public float DamageMultiplier => damageMultiplier;
    public float FireRateMultiplier => fireRateMultiplier;
    public int BonusProjectiles => bonusProjectiles;

    public event Action StatsChanged;

    private void Awake()
    {
        Debug.Log("[PlayerStats] Initialized.");
    }

    public void AddMaxHealth(float amount)
    {
        maxHealthBonus += amount;

        Debug.Log(
            $"[PlayerStats] Max Health Bonus = {maxHealthBonus}");

        StatsChanged?.Invoke();
    }

    public void AddMoveSpeedMultiplier(float amount)
    {
        moveSpeedMultiplier += amount;

        Debug.Log(
            $"[PlayerStats] Move Speed Multiplier = {moveSpeedMultiplier}");

        StatsChanged?.Invoke();
    }

    public void AddDamageMultiplier(float amount)
    {
        damageMultiplier += amount;

        Debug.Log(
            $"[PlayerStats] Damage Multiplier = {damageMultiplier}");

        StatsChanged?.Invoke();
    }

    public void AddFireRateMultiplier(float amount)
    {
        fireRateMultiplier += amount;

        Debug.Log(
            $"[PlayerStats] Fire Rate Multiplier = {fireRateMultiplier}");

        StatsChanged?.Invoke();
    }

    public void AddProjectileCount(int amount)
    {
        bonusProjectiles += amount;

        Debug.Log(
            $"[PlayerStats] Bonus Projectiles = {bonusProjectiles}");

        StatsChanged?.Invoke();
    }
}
