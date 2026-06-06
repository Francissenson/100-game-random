using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles sword attack hitbox activation.
/// </summary>
public sealed class SwordHitbox : MonoBehaviour
{
    [SerializeField]
    private Collider2D hitboxCollider;

    [SerializeField]
    private float activeDuration = 0.15f;

    [SerializeField]
    private int damage = 25;

    private readonly HashSet<IDamageable> damagedTargets = new();

    private Coroutine attackRoutine;

    private void Awake()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
        }
    }

    public void ActivateHitbox()
    {
        damagedTargets.Clear();

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        attackRoutine = StartCoroutine(ActivateRoutine());
    }

    private IEnumerator ActivateRoutine()
    {
        hitboxCollider.enabled = true;

        yield return new WaitForSeconds(activeDuration);

        hitboxCollider.enabled = false;

        attackRoutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable == null)
        {
            return;
        }

        if (damagedTargets.Contains(damageable))
        {
            return;
        }

        damagedTargets.Add(damageable);

        damageable.TakeDamage(damage);
    }
}