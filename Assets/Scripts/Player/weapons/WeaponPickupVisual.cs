using UnityEngine;

public class WeaponPickupVisual : MonoBehaviour
{
    [Header("Float")]
    [SerializeField] private float floatHeight = 0.15f;
    [SerializeField] private float floatSpeed = 2f;

    [Header("Shadow")]
    [SerializeField] private Transform shadow;

    [SerializeField] private float minShadowScale = 0.75f;
    [SerializeField] private float maxShadowScale = 1f;

    private Vector3 startLocalPosition;
    private Vector3 shadowStartScale;

    private void Start()
    {
        startLocalPosition = transform.localPosition;

        if (shadow != null)
        {
            shadowStartScale = shadow.localScale;
        }
    }

    private void Update()
    {
        float wave = (Mathf.Sin(Time.time * floatSpeed) + 1f) * 0.5f;

        float yOffset = wave * floatHeight;

        transform.localPosition =
            startLocalPosition +
            Vector3.up * yOffset;

        if (shadow != null)
        {
            float scaleMultiplier =
                Mathf.Lerp(maxShadowScale,
                           minShadowScale,
                           wave);

            shadow.localScale =
                shadowStartScale * scaleMultiplier;
        }
    }
}