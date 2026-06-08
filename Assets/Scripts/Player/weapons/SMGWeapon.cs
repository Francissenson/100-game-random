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

        Debug.Log(
            $"[SMGWeapon] Damage: {finalDamage}");

        Projectile projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation);

        projectile.Initialize(
            firePoint.right,
            finalDamage,
            projectileSpeed,
            projectileLifetime);
    }
}