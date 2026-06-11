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

    [Header("Feedback")]
    [SerializeField]
    private DamageNumber damageNumberPrefab;

    [SerializeField]
    private Vector3 damageNumberOffset =
        new Vector3(0f, 0.8f, 0f);

    [SerializeField]
    private Vector3 healthBarOffset =
        new Vector3(0f, 1.1f, 0f);

    private int currentHealth;
    private bool isDead;
    private WorldHealthBar healthBar;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    public event Action<EnemyHealth> OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;

        healthBar =
            WorldHealthBar.Create(
                transform,
                healthBarOffset,
                new Color(1f, 0.25f, 0.08f, 1f),
                1f,
                0.1f);

        healthBar.SetValue(
            currentHealth,
            maxHealth);

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
        currentHealth =
            Mathf.Max(
                currentHealth,
                0);

        ShowDamageNumber(
            damage);

        healthBar?.SetValue(
            currentHealth,
            maxHealth);

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

    private void OnDestroy()
    {
        if (healthBar != null)
        {
            Destroy(
                healthBar.gameObject);
        }
    }

    private void ShowDamageNumber(
        int damage)
    {
        DamageNumber damageNumber;

        if (damageNumberPrefab != null)
        {
            damageNumber =
                Instantiate(
                    damageNumberPrefab,
                    transform.position + damageNumberOffset,
                    Quaternion.identity);
        }
        else
        {
            GameObject damageNumberObject =
                new GameObject("DamageNumber");

            damageNumberObject.transform.position =
                transform.position + damageNumberOffset;

            damageNumber =
                damageNumberObject.AddComponent<DamageNumber>();
        }

        damageNumber.Show(
            damage);
    }
}
