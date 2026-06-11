using System.Collections;
using UnityEngine;

/// <summary>
/// Basic melee sword weapon.
/// </summary>
public sealed class SwordWeapon : Weapon
{
    [Header("Sword")]
    [SerializeField]
    private SwordHitbox swordHitbox;

    [Header("Swing")]
    [SerializeField]
    private float swingDuration = 0.18f;

    [SerializeField]
    private float swingStartAngle = -65f;

    [SerializeField]
    private float swingEndAngle = 65f;

    private Coroutine swingRoutine;
    private Quaternion restingLocalRotation;

    protected override void Awake()
    {
        base.Awake();

        restingLocalRotation = transform.localRotation;
    }

    private void OnDisable()
    {
        if (swingRoutine != null)
        {
            StopCoroutine(swingRoutine);
            swingRoutine = null;
        }

        transform.localRotation = restingLocalRotation;
    }

    protected override void PerformAttack()
    {
        Debug.Log("Sword Attack");

        PlaySwing();

        if (swordHitbox != null)
        {
            swordHitbox.ActivateHitbox();
        }
    }

    private void PlaySwing()
    {
        if (swingRoutine != null)
        {
            StopCoroutine(swingRoutine);
        }

        swingRoutine = StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, swingDuration);

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float easedProgress = Mathf.Sin(progress * Mathf.PI * 0.5f);
            float angle = Mathf.Lerp(swingStartAngle, swingEndAngle, easedProgress);

            transform.localRotation =
                restingLocalRotation *
                Quaternion.Euler(0f, 0f, angle);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localRotation = restingLocalRotation;
        swingRoutine = null;
    }
}
