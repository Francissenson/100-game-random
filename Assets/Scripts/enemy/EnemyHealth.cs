using System;
using UnityEngine;

/// <summary>
/// Handles enemy health and death.
/// </summary>
public sealed class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField]
    private int maxHealth = 50;

    private int currentHealth;
    private bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    public event Action<EnemyHealth> OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;

        Debug.Log($"{name} EnemyHealth Awake");
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"{name} TakeDamage Called");

        if (isDead)
        {
            Debug.LogWarning($"{name} already dead");
            return;
        }

        if (damage <= 0)
        {
            Debug.LogWarning($"{name} invalid damage: {damage}");
            return;
        }

        currentHealth -= damage;

        Debug.Log(
            $"{name} took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log($"{name} died.");

        OnDeath?.Invoke(this);

        Debug.Log($"{name} death event fired.");

        Destroy(gameObject);
    }
}