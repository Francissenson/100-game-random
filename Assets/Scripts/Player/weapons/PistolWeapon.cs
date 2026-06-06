using UnityEngine;

public class PistolWeapon : Weapon
{
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;

    [Header("Fire Point")]
    [SerializeField] private Transform firePoint;

    protected override void PerformAttack()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Projectile Prefab Missing");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning("Fire Point Missing");
            return;
        }

        Projectile projectile =
            Instantiate(
                projectilePrefab,
                firePoint.position,
                firePoint.rotation);

        projectile.Initialize(firePoint.right);
    }
}