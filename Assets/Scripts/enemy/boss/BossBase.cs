using System.Collections;
using UnityEngine;

public abstract class BossBase : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] protected float detectionRange = 25f;

    [Header("References")]
    [SerializeField] protected Transform visuals;
    [SerializeField] private float stunDuration = 0.15f;

    [Header("Animation")]
    [SerializeField] private string idleState = "Base Layer.idle";
    [SerializeField] private string walkState = "Base Layer.walk";
    [SerializeField] private string attackState = "Base Layer.attack";
    [SerializeField] private string dieState = "Base Layer.die";

    protected Transform player;
    private bool stunned;
    private Coroutine stunRoutine;
    private Animator animator;
    private string currentAnimationState;
    private bool isDying;

    protected virtual void Awake()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;

            Debug.Log(
                $"[{GetType().Name}] Player found.");
        }
        else
        {
            Debug.LogError(
                $"[{GetType().Name}] Player not found.");
        }
    }

    protected virtual void Start()
    {
        animator =
            GetComponentInChildren<Animator>(true);

        if (animator == null)
        {
            Debug.LogError(
                $"[{GetType().Name}] Boss animator missing.");
            return;
        }

        SetAnimation(
            idleState,
            true);
    }

    protected virtual void Update()
    {
        if (player == null)
        {
            return;
        }

        HandleVisualFlip();
    }

    protected virtual void HandleVisualFlip()
    {
        if (visuals == null)
        {
            return;
        }

        Vector3 scale =
            visuals.localScale;

        scale.x =
            player.position.x < transform.position.x
                ? -Mathf.Abs(scale.x)
                : Mathf.Abs(scale.x);

        visuals.localScale = scale;
    }

    protected void SetIdleAnimation()
    {
        SetAnimation(idleState);
    }

    protected void SetWalkAnimation()
    {
        if (!isDying)
        {
            SetAnimation(walkState);
        }
    }

    protected void SetAttackAnimation()
    {
        if (!isDying)
        {
            SetAnimation(attackState, true);
        }
    }

    public void PlayDeathAnimation()
    {
        if (isDying)
        {
            return;
        }

        isDying = true;
        SetAnimation(
            dieState,
            true);
    }

    public void Stun(float duration)
    {
        float stunTime =
            duration > 0f
                ? duration
                : stunDuration;

        if (stunRoutine != null)
        {
            StopCoroutine(
                stunRoutine);
        }

        stunRoutine =
            StartCoroutine(
                StunRoutine(
                    stunTime));
    }

    protected bool IsStunned()
    {
        return stunned;
    }

    protected float DistanceToPlayer()
    {
        if (player == null)
        {
            return float.MaxValue;
        }

        return Vector2.Distance(
            transform.position,
            player.position);
    }

    private void SetAnimation(
        string stateName,
        bool force = false)
    {
        if (animator == null ||
            string.IsNullOrWhiteSpace(stateName))
        {
            return;
        }

        if (!force &&
            currentAnimationState == stateName)
        {
            return;
        }

        animator.Play(
            stateName,
            0,
            0f);

        currentAnimationState = stateName;
    }

    private IEnumerator StunRoutine(
        float duration)
    {
        stunned = true;

        yield return new WaitForSeconds(
            duration);

        stunned = false;
        stunRoutine = null;
    }
}
