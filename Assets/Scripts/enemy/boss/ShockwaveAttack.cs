using UnityEngine;

public sealed class ShockwaveAttack : MonoBehaviour
{
    [SerializeField] private float radius = 3f;

    [SerializeField] private int damage = 20;

    public void TriggerShockwave()
    {
        Debug.Log(
            "[ShockwaveAttack] Triggered.");

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                transform.position,
                radius);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Player"))
            {
                continue;
            }

            IDamageable damageable =
                hit.GetComponent<IDamageable>();

            if (damageable == null)
            {
                continue;
            }

            damageable.TakeDamage(
                damage);

            Debug.Log(
                $"[ShockwaveAttack] Hit Player For {damage}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            radius);
    }
}