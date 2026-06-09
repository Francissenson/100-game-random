using UnityEngine;

public class SMGWeapon : Weapon
{
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;

    [SerializeField] private Transform firePoint;

    [Header("Projectile Stats")]
    [SerializeField] private int projectileDamage = 3;

    [SerializeField] private float projectileSpeed = 25f;

    [SerializeField] private float projectileLifetime = 2f;

    [Header("Bonus Projectile")]
    [SerializeField] private float projectileSpacing = 0.15f;

    protected override void PerformAttack()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning(
                "[SMGWeapon] Missing Projectile Prefab or Fire Point.");

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
            $"[SMGWeapon] Damage: {finalDamage} | Projectiles: {projectileCount}");

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