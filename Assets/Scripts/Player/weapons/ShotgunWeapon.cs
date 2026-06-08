using UnityEngine;

public class ShotgunWeapon : Weapon
{
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;

    [SerializeField] private Transform firePoint;

    [Header("Projectile Stats")]
    [SerializeField] private int projectileDamage = 4;

    [SerializeField] private float projectileSpeed = 15f;

    [SerializeField] private float projectileLifetime = 1.5f;

    [Header("Shotgun Settings")]
    [SerializeField] private int pelletCount = 5;

    [SerializeField] private float spreadAngle = 20f;

    protected override void PerformAttack()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning(
                "[ShotgunWeapon] Missing Projectile Prefab.");

            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning(
                "[ShotgunWeapon] Missing Fire Point.");

            return;
        }

        int finalDamage =
            GetModifiedDamage(projectileDamage);

        Debug.Log(
            $"[ShotgunWeapon] Pellet Damage: {finalDamage}");

        float halfSpread = spreadAngle * 0.5f;

        for (int i = 0; i < pelletCount; i++)
        {
            float angle;

            if (pelletCount == 1)
            {
                angle = 0f;
            }
            else
            {
                angle = Mathf.Lerp(
                    -halfSpread,
                    halfSpread,
                    i / (float)(pelletCount - 1));
            }

            Quaternion pelletRotation =
                firePoint.rotation *
                Quaternion.Euler(0f, 0f, angle);

            Projectile projectile =
                Instantiate(
                    projectilePrefab,
                    firePoint.position,
                    pelletRotation);

            Vector2 direction =
                pelletRotation * Vector2.right;

            projectile.Initialize(
                direction,
                finalDamage,
                projectileSpeed,
                projectileLifetime);
        }
    }
}