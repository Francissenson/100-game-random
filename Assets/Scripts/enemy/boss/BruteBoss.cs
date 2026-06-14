using System.Collections;
using UnityEngine;

public sealed class BruteBoss : BossBase
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [SerializeField] private float phase2MoveSpeed = 3.5f;

    [SerializeField] private float attackRange = 2f;

    [Header("Attack")]
    [SerializeField] private Transform attackPivot;

    [SerializeField] private Transform attackPoint;

    [SerializeField] private float attackRadius = 2f;

    [SerializeField] private int damage = 30;

    [SerializeField] private int phase2Damage = 45;

    [SerializeField] private float rotationSpeed = 360f;

    [SerializeField] private float windupTime = 1f;

    [SerializeField] private float phase2WindupTime = 0.5f;

    [SerializeField] private float recoveryTime = 1f;

    [Header("Phase 2")]
    [SerializeField] private ShockwaveAttack shockwaveAttack;

    [Header("Health")]
    [SerializeField] private EnemyHealth enemyHealth;

    private Rigidbody2D rb;

    private BossState currentState =
        BossState.Idle;

    private BossPhase currentPhase =
        BossPhase.Phase1;

    private bool attackRoutineRunning;

    private bool phase2Triggered;

    private Vector2 lockedTargetPosition;

    private float currentMoveSpeed;

    private int currentDamage;

    private float currentWindupTime;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();

        currentMoveSpeed = moveSpeed;
        currentDamage = damage;
        currentWindupTime = windupTime;

        Debug.Log(
            "[BruteBoss] Initialized.");
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (player == null)
        {
            return;
        }

        if (IsStunned())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        CheckPhaseTransition();

        switch (currentState)
        {
            case BossState.Idle:
            case BossState.Chasing:
                HandleMovement();
                break;
        }
    }

    private void CheckPhaseTransition()
    {
        if (phase2Triggered)
        {
            return;
        }

        if (enemyHealth == null)
        {
            return;
        }

        float healthPercent =
            (float)enemyHealth.CurrentHealth /
            enemyHealth.MaxHealth;

        if (healthPercent <= 0.5f)
        {
            EnterPhase2();
        }
    }

    private void EnterPhase2()
    {
        phase2Triggered = true;

        currentPhase =
            BossPhase.Phase2;

        currentMoveSpeed =
            phase2MoveSpeed;

        currentDamage =
            phase2Damage;

        currentWindupTime =
            phase2WindupTime;

        Debug.Log(
            "[BruteBoss] PHASE 2!");
    }

    private void HandleMovement()
    {
        if (IsStunned())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance =
            DistanceToPlayer();

        if (distance > attackRange)
        {
            currentState =
                BossState.Chasing;

            SetWalkAnimation();

            Vector2 direction =
                (player.position - transform.position).normalized;

            rb.linearVelocity =
                direction * currentMoveSpeed;
        }
        else
        {
            rb.linearVelocity =
                Vector2.zero;
            SetIdleAnimation();

            if (!attackRoutineRunning)
            {
                StartCoroutine(
                    AttackRoutine());
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        attackRoutineRunning = true;

        currentState =
            BossState.Windup;
        SetAttackAnimation();

        lockedTargetPosition =
            player.position;

        Debug.Log(
            "[BruteBoss] Attack Windup.");

        yield return StartCoroutine(
            RotateToTarget());

        yield return WaitRespectingStun(
            currentWindupTime);

        currentState =
            BossState.Attacking;

        PerformAttack();

        if (currentPhase == BossPhase.Phase2)
        {
            yield return WaitRespectingStun(
                0.25f);

            yield return StartCoroutine(
                TriggerShockwave());
        }

        currentState =
            BossState.Recovery;
        SetIdleAnimation();

        Debug.Log(
            "[BruteBoss] Recovery.");

        yield return WaitRespectingStun(
            recoveryTime);

        currentState =
            BossState.Chasing;

        attackRoutineRunning = false;
    }

    private IEnumerator RotateToTarget()
    {
        if (attackPivot == null)
        {
            yield break;
        }

        Vector2 direction =
            lockedTargetPosition -
            (Vector2)attackPivot.position;

        float targetAngle =
            Mathf.Atan2(
                direction.y,
                direction.x) *
            Mathf.Rad2Deg;

        Quaternion targetRotation =
            Quaternion.Euler(
                0f,
                0f,
                targetAngle);

        while (Quaternion.Angle(
                   attackPivot.rotation,
                   targetRotation) > 1f)
        {
            if (IsStunned())
            {
                rb.linearVelocity = Vector2.zero;
                yield return null;
                continue;
            }

            attackPivot.rotation =
                Quaternion.RotateTowards(
                    attackPivot.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime);

            yield return null;
        }
    }

    private IEnumerator WaitRespectingStun(
        float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (IsStunned())
            {
                rb.linearVelocity = Vector2.zero;
                yield return null;
                continue;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void PerformAttack()
    {
        Debug.Log(
            "[BruteBoss] Slam Attack.");

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRadius);

        foreach (Collider2D hit in hits)
        {
            PlayerHealth playerHealth =
                hit.GetComponentInParent<PlayerHealth>();

            if (playerHealth == null)
            {
                continue;
            }

            playerHealth.TakeDamage(
                currentDamage);

            Debug.Log(
                $"[BruteBoss] Hit Player For {currentDamage}");

            break;
        }
    }

    private IEnumerator TriggerShockwave()
    {
        if (shockwaveAttack == null)
        {
            Debug.LogWarning(
                "[BruteBoss] ShockwaveAttack missing.");

            yield break;
        }

        Debug.Log(
            "[BruteBoss] Shockwave!");

        yield return StartCoroutine(
            shockwaveAttack.TriggerShockwave());
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius);
    }
}
