using System;
using UnityEngine;

/// <summary>
/// Handles player health, healing, and death.
/// </summary>
public sealed class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField]
    private int maxHealth = 100;

    private int currentHealth;
    private bool isDead;

    /// <summary>
    /// Raised whenever health changes.
    /// CurrentHealth, MaxHealth
    /// </summary>
    public event Action<int, int> HealthChanged;

    /// <summary>
    /// Raised when the player dies.
    /// </summary>
    public event Action Died;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Applies damage to the player.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        if (damage <= 0)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Heals the player.
    /// </summary>
    public void Heal(int amount)
    {
        if (isDead)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Fully restores health.
    /// </summary>
    public void RestoreFullHealth()
    {
        if (isDead)
        {
            return;
        }

        currentHealth = maxHealth;

        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Increases maximum health and grants the added health immediately.
    /// </summary>
    public void IncreaseMaxHealth(int amount)
    {
        if (isDead)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        maxHealth += amount;
        currentHealth += amount;

        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Died?.Invoke();
    }
}