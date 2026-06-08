using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyHealth))]
public class ShooterEnemy : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float shootRange = 6f;

    [Header("Weapon")]
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private Transform firePoint;

    [Header("Projectile")]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float projectileLifetime = 3f;
    [SerializeField] private int projectileDamage = 10;

    [Header("Aim")]
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float angleTolerance = 2f;

    [Header("Timing")]
    [SerializeField] private float recoveryTime = 1f;

    private bool isAttacking;
    private bool isRecovering;

    private Vector2 lockedTargetPosition;

    protected override void Start()
    {
        base.Start();

        Debug.Log($"{name} Shooter Initialized");
    }

    private void FixedUpdate()
    {
        if (!HasTarget())
            return;

        if (!IsAlive())
            return;

        if (isAttacking)
            return;

        if (isRecovering)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                target.position);

        if (distance > shootRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StartCoroutine(ShootRoutine());
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction =
            (target.position - transform.position).normalized;

        Vector2 newPosition =
            rb.position +
            direction * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }

    private IEnumerator ShootRoutine()
    {
        isAttacking = true;

        lockedTargetPosition = target.position;

        Debug.Log("Target Position Locked");

        yield return RotateWeaponToLockedTarget();

        Shoot();

        Debug.Log("Shooter Fired");

        isAttacking = false;
        isRecovering = true;

        Debug.Log("Shooter Recovery Started");

        yield return new WaitForSeconds(recoveryTime);

        isRecovering = false;

        Debug.Log("Shooter Recovery Ended");
    }

    private IEnumerator RotateWeaponToLockedTarget()
    {
        if (weaponPivot == null)
        {
            Debug.LogError("WeaponPivot Missing");
            yield break;
        }

        Debug.Log("Shooter Started Aiming");

        while (true)
        {
            Vector2 direction =
                lockedTargetPosition -
                (Vector2)weaponPivot.position;

            float targetAngle =
                Mathf.Atan2(direction.y, direction.x)
                * Mathf.Rad2Deg;

            float currentAngle =
                weaponPivot.eulerAngles.z;

            float newAngle =
                Mathf.MoveTowardsAngle(
                    currentAngle,
                    targetAngle,
                    rotationSpeed * Time.deltaTime);

            weaponPivot.rotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    newAngle);

            float difference =
                Mathf.Abs(
                    Mathf.DeltaAngle(
                        newAngle,
                        targetAngle));

            if (difference <= angleTolerance)
            {
                Debug.Log("Shooter Aim Complete");
                break;
            }

            yield return null;
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab Missing");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("FirePoint Missing");
            return;
        }

        Vector2 direction =
            (lockedTargetPosition -
             (Vector2)firePoint.position).normalized;

        EnemyProjectile projectile =
            Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.identity);

        projectile.Initialize(
            direction,
            projectileSpeed,
            projectileDamage,
            projectileLifetime);

        Debug.Log("Projectile Spawned");
    }
}