using System.Collections;
using UnityEngine;

public sealed class ShockwaveAttack : MonoBehaviour
{
    [SerializeField] private float radius = 3f;

    [SerializeField] private int damage = 20;

    [SerializeField] private ShockwaveTelegraph telegraph;

    [SerializeField] private float warningTime = 0.75f;

    public IEnumerator TriggerShockwave()
    {
        Debug.Log(
            "[ShockwaveAttack] Warning.");

        if (telegraph != null)
        {
            yield return StartCoroutine(
                telegraph.ShowTelegraph());
        }
        else
        {
            yield return new WaitForSeconds(
                warningTime);
        }

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
                hit.GetComponentInParent<IDamageable>();

            if (damageable == null)
            {
                continue;
            }

            damageable.TakeDamage(
                damage);

            Debug.Log(
                $"[ShockwaveAttack] Hit Player For {damage}");
        }

        if (telegraph != null)
        {
            telegraph.HideTelegraph();
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