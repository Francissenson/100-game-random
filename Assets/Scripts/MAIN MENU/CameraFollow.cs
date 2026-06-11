using UnityEngine;

public sealed class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset =
        new Vector3(0f, 0f, -10f);

    [SerializeField]
    private float followSpeed = 10f;

    [Header("Shake")]
    [SerializeField]
    private float shakeIntensity = 0.18f;

    [SerializeField]
    private float shakeDuration = 0.12f;

    private Transform target;
    private float shakeTimer;
    private float currentShakeIntensity;

    public void Shake()
    {
        Shake(
            shakeIntensity);
    }

    public void Shake(
        float intensity)
    {
        currentShakeIntensity =
            Mathf.Max(
                intensity,
                0f);

        shakeTimer =
            Mathf.Max(
                shakeDuration,
                0f);
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            GameObject player =
                GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                return;
            }
        }

        Vector3 desiredPosition =
            target.position + offset;

        if (shakeTimer > 0f)
        {
            Vector2 shakeOffset =
                Random.insideUnitCircle * currentShakeIntensity;

            desiredPosition +=
                new Vector3(
                    shakeOffset.x,
                    shakeOffset.y,
                    0f);

            shakeTimer -= Time.deltaTime;
        }

        transform.position =
            Vector3.Lerp(
                transform.position,
                desiredPosition,
                followSpeed * Time.deltaTime);
    }
}
