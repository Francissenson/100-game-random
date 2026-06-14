using System;
using System.Collections;
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

    [SerializeField]
    private float stunDuration = 0.15f;

    [SerializeField]
    private float damageFlashDuration = 0.08f;

    [SerializeField]
    private Color damageFlashColor = Color.red;

    [SerializeField]
    private float bossDeathDelay = 1.05f;

    private int currentHealth;
    private bool isDead;
    private WorldHealthBar healthBar;
    private SpriteRenderer[] spriteRenderers;
    private Color[] originalSpriteColors;
    private Coroutine damageFlashRoutine;
    private bool isFlashing;
    private EnemyBase enemyBase;
    private BossBase bossBase;

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

        spriteRenderers =
            GetComponentsInChildren<SpriteRenderer>(
                true);

        enemyBase =
            GetComponentInParent<EnemyBase>();

        bossBase =
            GetComponentInParent<BossBase>();

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

        AudioManager.Instance?.PlayEnemyHit();

        ApplyHitReaction();

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

        AudioManager.Instance?.PlayEnemyDeath();

        if (bossBase != null)
        {
            StartCoroutine(
                BossDeathRoutine());
            return;
        }

        OnDeath?.Invoke(this);

        Debug.Log($"{name} death event fired.");

        Destroy(gameObject);
    }

    private IEnumerator BossDeathRoutine()
    {
        bossBase.PlayDeathAnimation();

        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>(true);

        foreach (Collider2D collider in colliders)
        {
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        Rigidbody2D rb =
            GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        yield return new WaitForSeconds(
            bossDeathDelay);

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

    private void ApplyHitReaction()
    {
        if (!isFlashing)
        {
            CacheOriginalSpriteColors();
        }

        SetSpriteColors(
            damageFlashColor);

        if (damageFlashRoutine != null)
        {
            StopCoroutine(
                damageFlashRoutine);
        }

        damageFlashRoutine =
            StartCoroutine(
                DamageFlashRoutine());

        enemyBase?.Stun(
            stunDuration);
        bossBase?.Stun(
            stunDuration);
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (spriteRenderers == null ||
            spriteRenderers.Length == 0)
        {
            damageFlashRoutine = null;
            isFlashing = false;
            yield break;
        }

        yield return new WaitForSeconds(
            damageFlashDuration);

        RestoreOriginalSpriteColors();

        damageFlashRoutine = null;
        isFlashing = false;
    }

    private void CacheOriginalSpriteColors()
    {
        if (spriteRenderers == null ||
            spriteRenderers.Length == 0)
        {
            return;
        }

        originalSpriteColors =
            new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            SpriteRenderer spriteRenderer =
                spriteRenderers[i];

            if (spriteRenderer == null)
            {
                continue;
            }

            originalSpriteColors[i] =
                spriteRenderer.color;
        }

        isFlashing = true;
    }

    private void SetSpriteColors(
        Color color)
    {
        if (spriteRenderers == null ||
            spriteRenderers.Length == 0)
        {
            return;
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            SpriteRenderer spriteRenderer =
                spriteRenderers[i];

            if (spriteRenderer == null)
            {
                continue;
            }

            spriteRenderer.color = color;
        }
    }

    private void RestoreOriginalSpriteColors()
    {
        if (spriteRenderers == null ||
            spriteRenderers.Length == 0 ||
            originalSpriteColors == null)
        {
            return;
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            SpriteRenderer spriteRenderer =
                spriteRenderers[i];

            if (spriteRenderer == null)
            {
                continue;
            }

            spriteRenderer.color =
                originalSpriteColors[i];
        }
    }
}
