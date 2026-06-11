using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Player;

public sealed class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private float invulnerabilityDuration = 0.5f;

    [Header("Feedback")]
    [SerializeField]
    private Vector3 healthBarOffset =
        new Vector3(0f, 1.35f, 0f);

    [SerializeField]
    private float damageFlashDuration = 0.08f;

    [SerializeField]
    private Color damageFlashColor = Color.red;

    [SerializeField]
    private float cameraShakeIntensity = 0.18f;

    private int currentHealth;
    private int baseMaxHealth;
    private PlayerStats playerStats;

    private bool invulnerable;
    private WorldHealthBar healthBar;
    private SpriteRenderer[] spriteRenderers;
    private Coroutine damageFlashRoutine;
    private CameraFollow cameraFollow;
    public bool IsDead { get; private set; }

    public int CurrentHealth => currentHealth;

    public int MaxHealth => maxHealth;

    private void Awake()
    {
        baseMaxHealth = maxHealth;
        playerStats =
            GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.StatsChanged += HandleStatsChanged;
        }

        ApplyMaxHealthFromStats(false);

        currentHealth = maxHealth;
        spriteRenderers =
            GetComponentsInChildren<SpriteRenderer>(
                true);

        healthBar =
            WorldHealthBar.Create(
                transform,
                healthBarOffset,
                new Color(0.25f, 1f, 0.25f, 1f),
                1.6f,
                0.16f);

        healthBar.SetValue(
            currentHealth,
            maxHealth);

    }

    private void HandleStatsChanged()
    {
        ApplyMaxHealthFromStats(true);
    }

    private void ApplyMaxHealthFromStats(bool preserveMissingHealth)
    {
        int previousMaxHealth = maxHealth;
        int missingHealth =
            Mathf.Max(0, previousMaxHealth - currentHealth);

        int bonusHealth =
            playerStats != null
                ? Mathf.RoundToInt(playerStats.MaxHealthBonus)
                : 0;

        maxHealth =
            Mathf.Max(1, baseMaxHealth + bonusHealth);

        if (!preserveMissingHealth)
        {
            return;
        }

        currentHealth =
            Mathf.Clamp(
                maxHealth - missingHealth,
                0,
                maxHealth);

        healthBar?.SetValue(
            currentHealth,
            maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (invulnerable || IsDead)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth =
            Mathf.Max(
                currentHealth,
                0);

        healthBar?.SetValue(
            currentHealth,
            maxHealth);

        Debug.Log(
            $"[PlayerHealth] Took {damage} Damage. HP = {currentHealth}");

        PlayDamageFeedback();

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            Die();

            return;
        }

        StartCoroutine(
            InvulnerabilityRoutine());
    }

    private void PlayDamageFeedback()
    {
        if (cameraFollow == null)
        {
            cameraFollow =
                FindFirstObjectByType<CameraFollow>();
        }

        if (cameraFollow != null)
        {
            cameraFollow.Shake(
                cameraShakeIntensity);
        }

        if (damageFlashRoutine != null)
        {
            StopCoroutine(
                damageFlashRoutine);
        }

        damageFlashRoutine =
            StartCoroutine(
                DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            yield break;
        }

        Color[] originalColors =
            new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
            {
                continue;
            }

            originalColors[i] =
                spriteRenderers[i].color;
            spriteRenderers[i].color =
                damageFlashColor;
        }

        yield return new WaitForSeconds(
            damageFlashDuration);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
            {
                continue;
            }

            spriteRenderers[i].color =
                originalColors[i];
        }

        damageFlashRoutine = null;
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        invulnerable = true;

        yield return new WaitForSeconds(
            invulnerabilityDuration);

        invulnerable = false;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healthBar?.SetValue(
            currentHealth,
            maxHealth);
    }

    private void Die()
    {
        if (IsDead)
        {
            return;
        }

        StartCoroutine(
            DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        IsDead = true;

        Debug.Log(
            "[PlayerHealth] Player Died");

        PlayerCharacter playerCharacter =
            GetComponent<PlayerCharacter>();

        if (playerCharacter != null)
        {
            playerCharacter.PlayDeath();
        }

        if (RunResultManager.Instance != null)
        {
            RunResultManager.Instance.SetGameOver();
        }

        // Disable gameplay immediately
        MonoBehaviour[] behaviours =
            GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour == this)
            {
                continue;
            }

            behaviour.enabled = false;
        }

        Rigidbody2D rb =
            GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        yield return new WaitForSeconds(1.2f);

        if (RunResultManager.Instance != null)
        {
            RunResultManager.Instance.SetGameOver();
        }

        SpriteRenderer sprite =
            GetComponent<SpriteRenderer>();

        if (sprite != null)
        {
            Color color =
                sprite.color;

            float duration = 1f;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                color.a =
                    Mathf.Lerp(
                        1f,
                        0f,
                        timer / duration);

                sprite.color = color;

                yield return null;
            }
        }

        SceneManager.LoadScene(
            "EndRunScene");
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.StatsChanged -= HandleStatsChanged;
        }

        if (healthBar != null)
        {
            Destroy(
                healthBar.gameObject);
        }
    }
}
