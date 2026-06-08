using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyHealth))]
public class TankEnemy : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float attackRange = 1.5f;

    [Header("Attack")]
    [SerializeField] private Transform attackPivot;
    [SerializeField] private Transform attackPoint;

    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private int attackDamage = 25;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float angleTolerance = 2f;

    [Header("Timing")]
    [SerializeField] private float attackWindup = 0.75f;
    [SerializeField] private float attackRecovery = 1.5f;

    [Header("Swing")]
    [SerializeField] private float swingAngle = 120f;
    [SerializeField] private float swingDuration = 0.3f;

    private bool isAttacking;

    private Vector2 lockedTargetPosition;

    protected override void Start()
    {
        base.Start();

        Debug.Log($"{name} Tank Initialized");
    }

    private void FixedUpdate()
    {
        if (!HasTarget())
            return;

        if (!IsAlive())
            return;

        if (isAttacking)
            return;

        float distance =
            Vector2.Distance(
                rb.position,
                target.position);

        if (distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StartCoroutine(AttackRoutine());
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

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        lockedTargetPosition = target.position;

        Debug.Log("Tank Target Position Locked");

        yield return RotateAttackPivotToTarget();

        Debug.Log("Tank Aim Complete");

        yield return new WaitForSeconds(attackWindup);

        Debug.Log("Tank Windup Complete");

        yield return SwingAttack();

        Debug.Log("Tank Swing Complete");

        yield return new WaitForSeconds(attackRecovery);

        Debug.Log("Tank Recovery Complete");

        isAttacking = false;
    }

    private IEnumerator RotateAttackPivotToTarget()
    {
        if (attackPivot == null)
        {
            Debug.LogError("AttackPivot Missing");
            yield break;
        }

        Debug.Log("Tank Started Aiming");

        while (true)
        {
            Vector2 direction =
                lockedTargetPosition -
                (Vector2)attackPivot.position;

            float targetAngle =
                Mathf.Atan2(
                    direction.y,
                    direction.x)
                * Mathf.Rad2Deg;

            float currentAngle =
                attackPivot.eulerAngles.z;

            float newAngle =
                Mathf.MoveTowardsAngle(
                    currentAngle,
                    targetAngle,
                    rotationSpeed * Time.deltaTime);

            attackPivot.rotation =
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
                break;
            }

            yield return null;
        }
    }

    private IEnumerator SwingAttack()
    {
        float centerAngle =
            attackPivot.eulerAngles.z;

        float startAngle =
            centerAngle - (swingAngle * 0.5f);

        float endAngle =
            centerAngle + (swingAngle * 0.5f);

        attackPivot.rotation =
            Quaternion.Euler(
                0f,
                0f,
                startAngle);

        bool damaged = false;

        float elapsed = 0f;

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / swingDuration;

            float angle =
                Mathf.LerpAngle(
                    startAngle,
                    endAngle,
                    t);

            attackPivot.rotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    angle);

            if (!damaged)
            {
                PerformAttack();
                damaged = true;
            }

            yield return null;
        }

        attackPivot.rotation =
            Quaternion.Euler(
                0f,
                0f,
                centerAngle);
    }

    private void PerformAttack()
    {
        Debug.Log("Tank Attack Triggered");

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.transform == transform)
                continue;

            if (hit.transform.IsChildOf(transform))
                continue;

            Debug.Log("Tank Hit: " + hit.name);

            if (!hit.CompareTag("Player"))
                continue;

            PlayerHealth playerHealth =
                hit.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                Debug.LogWarning("PlayerHealth Missing");
                continue;
            }

            playerHealth.TakeDamage(attackDamage);

            Debug.Log("Tank Damaged Player");

            break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius);
    }
}