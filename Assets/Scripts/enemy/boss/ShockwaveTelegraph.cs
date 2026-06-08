using System.Collections;
using UnityEngine;

public sealed class ShockwaveTelegraph : MonoBehaviour
{
    [SerializeField] private float duration = 0.75f;

    [SerializeField] private float targetScale = 8f;

    private SpriteRenderer spriteRenderer;

    private Vector3 initialScale;

    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();

        initialScale =
            Vector3.zero;

        transform.localScale =
            initialScale;

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    public IEnumerator ShowTelegraph()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        transform.localScale =
            Vector3.zero;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                timer / duration;

            float scale =
                Mathf.Lerp(
                    0f,
                    targetScale,
                    t);

            transform.localScale =
                new Vector3(
                    scale,
                    scale,
                    1f);

            yield return null;
        }

        transform.localScale =
            new Vector3(
                targetScale,
                targetScale,
                1f);
    }

    public void HideTelegraph()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        transform.localScale =
            Vector3.zero;
    }
}