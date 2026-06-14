using UnityEngine;

public class PistolWeapon : Weapon
{
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;

    [SerializeField] private Transform firePoint;

    [Header("Projectile Stats")]
    [SerializeField] private int projectileDamage = 10;

    [SerializeField] private float projectileSpeed = 20f;

    [SerializeField] private float projectileLifetime = 3f;

    [Header("Bonus Projectile")]
    [SerializeField] private float projectileSpacing = 0.20f;

    protected override void PerformAttack()
    {
        AudioManager.Instance?.PlayGunshot();

        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning(
                "[PistolWeapon] Missing Projectile Prefab or Fire Point.");

            return;
        }

        int finalDamage =
            GetModifiedDamage(projectileDamage);

        int projectileCount = 1;

        if (playerStats != null)
        {
            projectileCount +=
                playerStats.BonusProjectiles;
        }

        Debug.Log(
            $"[PistolWeapon] Damage: {finalDamage} | Projectiles: {projectileCount}");

        float startOffset =
            -((projectileCount - 1) * projectileSpacing) * 0.5f;

        for (int i = 0; i < projectileCount; i++)
        {
            Vector3 spawnOffset =
                firePoint.up *
                (startOffset + (i * projectileSpacing));

            Projectile projectile =
                Instantiate(
                    projectilePrefab,
                    firePoint.position + spawnOffset,
                    firePoint.rotation);

            projectile.Initialize(
                firePoint.right,
                finalDamage,
                projectileSpeed,
                projectileLifetime);
        }
    }
}
