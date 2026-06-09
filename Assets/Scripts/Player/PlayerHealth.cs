using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private float invulnerabilityDuration = 0.5f;

    private int currentHealth;

    private bool invulnerable;

    public bool IsDead { get; private set; }

    public int CurrentHealth => currentHealth;

    public int MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (invulnerable || IsDead)
        {
            return;
        }

        currentHealth -= damage;

        Debug.Log(
            $"[PlayerHealth] Took {damage} Damage. HP = {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            Die();

            return;
        }

        StartCoroutine(
            InvulnerabilityRoutine());
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

        yield return new WaitForSeconds(2f);

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
}