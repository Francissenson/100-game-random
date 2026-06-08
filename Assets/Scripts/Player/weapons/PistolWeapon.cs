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

    protected override void PerformAttack()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning(
                "[PistolWeapon] Missing Projectile Prefab or Fire Point.");

            return;
        }

        int finalDamage =
            GetModifiedDamage(projectileDamage);

        Debug.Log(
            $"[PistolWeapon] Damage: {finalDamage}");

        Projectile projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation);

        Vector2 direction = firePoint.right;

        projectile.Initialize(
            direction,
            finalDamage,
            projectileSpeed,
            projectileLifetime);
    }
}