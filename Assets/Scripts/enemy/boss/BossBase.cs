using UnityEngine;

public abstract class BossBase : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] protected float detectionRange = 25f;

    [Header("References")]
    [SerializeField] protected Transform visuals;

    protected Transform player;

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
}