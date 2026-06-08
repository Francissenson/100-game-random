using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyHealth))]
public class GruntEnemy : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackRange = 1.25f;

    [Header("Attack")]
    [SerializeField] private Transform attackPivot;
    [SerializeField] private Transform attackPoint;

    [SerializeField] private float attackRadius = 0.75f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Swing")]
    [SerializeField] private float swingAngle = 90f;
    [SerializeField] private float swingDuration = 0.15f;

    private bool isAttacking;
    private float nextAttackTime;

    private void FixedUpdate()
    {
        if (!HasTarget())
            return;

        if (!IsAlive())
            return;

        float distance = Vector2.Distance(
            rb.position,
            target.position);

        if (distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            TryAttack();
        }
    }

    private void MoveTowardsPlayer()
    {
        if (isAttacking)
            return;

        Vector2 currentPosition = rb.position;
        Vector2 playerPosition = target.position;

        Vector2 direction =
            (playerPosition - currentPosition).normalized;

        Vector2 newPosition =
            currentPosition +
            direction * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }

    private void TryAttack()
    {
        if (isAttacking)
            return;

        if (Time.time < nextAttackTime)
            return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        FaceAttackPivotTowardsPlayer();

        float centerAngle = attackPivot.eulerAngles.z;

        float startAngle =
            centerAngle - (swingAngle * 0.5f);

        float endAngle =
            centerAngle + (swingAngle * 0.5f);

        attackPivot.rotation =
            Quaternion.Euler(0f, 0f, startAngle);

        bool hasDamaged = false;

        float elapsed = 0f;

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / swingDuration;

            float angle = Mathf.LerpAngle(
                startAngle,
                endAngle,
                t);

            attackPivot.rotation =
                Quaternion.Euler(0f, 0f, angle);

            if (!hasDamaged)
            {
                PerformAttack();
                hasDamaged = true;
            }

            yield return null;
        }

        attackPivot.rotation =
            Quaternion.Euler(0f, 0f, centerAngle);

        nextAttackTime = Time.time + attackCooldown;

        isAttacking = false;
    }

    private void FaceAttackPivotTowardsPlayer()
    {
        if (target == null)
            return;

        Vector2 direction =
            target.position - attackPivot.position;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        attackPivot.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }

    private void PerformAttack()
    {
        Debug.Log("Enemy Attack Triggered");

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

            Debug.Log("Hit: " + hit.name);

            if (!hit.CompareTag("Player"))
                continue;

            PlayerHealth playerHealth =
                hit.GetComponent<PlayerHealth>();

            if (playerHealth == null)
                continue;

            Debug.Log("Player Damaged");

            playerHealth.TakeDamage(attackDamage);

            break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius);
    }
}