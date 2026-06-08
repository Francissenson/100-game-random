using UnityEngine;

public sealed class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    public int CurrentHealth => currentHealth;

    public int MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;

        Debug.Log(
            $"[PlayerHealth] Health = {currentHealth}");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log(
            $"[PlayerHealth] Took {damage} Damage. HP = {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log(
            $"[PlayerHealth] Healed {amount}. HP = {currentHealth}");
    }

    private void Die()
    {
        Debug.Log(
            "[PlayerHealth] Player Died");

        RunResultManager.Instance.SetGameOver();

        SceneLoader.Instance.LoadScene(
            "EndRunScene");
    }
}