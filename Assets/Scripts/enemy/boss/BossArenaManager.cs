using UnityEngine;

public sealed class BossArenaManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyHealth bossHealth;

    private void Start()
    {
        if (bossHealth == null)
        {
            Debug.LogError(
                "[BossArenaManager] BossHealth missing.");

            return;
        }

        bossHealth.OnDeath +=
            HandleBossDeath;

        Debug.Log(
            "[BossArenaManager] Initialized.");
    }

    private void OnDestroy()
    {
        if (bossHealth != null)
        {
            bossHealth.OnDeath -=
                HandleBossDeath;
        }
    }

    private void HandleBossDeath(
        EnemyHealth deadBoss)
    {
        Debug.Log(
            "[BossArenaManager] Boss Defeated!");

        Debug.Log(
            "[BossArenaManager] Victory!");
    }
}