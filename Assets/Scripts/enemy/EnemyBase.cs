using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Transform visuals;

    protected Transform target;
    protected EnemyHealth enemyHealth;
    protected Rigidbody2D rb;

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
}