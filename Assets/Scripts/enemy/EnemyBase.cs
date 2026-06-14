using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Transform visuals;
    [SerializeField] private float stunDuration = 0.15f;

    protected Transform target;
    protected EnemyHealth enemyHealth;
    protected Rigidbody2D rb;
    private bool stunned;
    private Coroutine stunRoutine;

    protected virtual void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;
        }
    }

    protected virtual void Update()
    {
        UpdateFacing();
    }

    private void UpdateFacing()
    {
        if (target == null)
            return;

        if (visuals == null)
            return;

        Vector3 scale = visuals.localScale;

        if (target.position.x < transform.position.x)
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = Mathf.Abs(scale.x);
        }

        visuals.localScale = scale;
    }

    protected Transform GetTarget()
    {
        return target;
    }

    protected bool HasTarget()
    {
        return target != null;
    }

    protected bool IsAlive()
    {
        return enemyHealth != null &&
               !enemyHealth.IsDead;
    }

    protected bool IsStunned()
    {
        return stunned;
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

    private IEnumerator StunRoutine(
        float duration)
    {
        stunned = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        yield return new WaitForSeconds(
            duration);

        stunned = false;
        stunRoutine = null;
    }
}
