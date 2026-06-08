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

    private PlayerStats playerStats;

    private readonly HashSet<IDamageable> damagedTargets = new();

    private Coroutine attackRoutine;

    private void Awake()
    {
        playerStats =
            GetComponentInParent<PlayerStats>();

        if (playerStats == null)
        {
            playerStats =
                FindFirstObjectByType<PlayerStats>();
        }

        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
        }
    }

    public void ActivateHitbox()
    {
        Debug.Log("[SwordHitbox] Activated");

        damagedTargets.Clear();

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        attackRoutine = StartCoroutine(ActivateRoutine());
    }

    private IEnumerator ActivateRoutine()
    {
        if (hitboxCollider == null)
        {
            Debug.LogError(
                "SwordHitbox is missing a Collider2D reference.");

            attackRoutine = null;

            yield break;
        }

        hitboxCollider.enabled = true;

        yield return new WaitForSeconds(activeDuration);

        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
        }

        attackRoutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(
              $"[SwordHitbox] Triggered: {other.name}");

        IDamageable damageable =
            other.GetComponentInParent<IDamageable>();

        if (damageable == null)
        {
            return;
        }

        if (damagedTargets.Contains(damageable))
        {
            return;
        }

        damagedTargets.Add(damageable);

        int finalDamage = damage;

        if (playerStats != null)
        {
            finalDamage =
                Mathf.RoundToInt(
                    damage *
                    playerStats.DamageMultiplier);
        }

        Debug.Log(
            $"[SwordHitbox] Damage: {finalDamage}");

        damageable.TakeDamage(finalDamage);
    }
}