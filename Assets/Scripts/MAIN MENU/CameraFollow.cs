using UnityEngine;

public sealed class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset =
        new Vector3(0f, 0f, -10f);

    [SerializeField]
    private float followSpeed = 10f;

    private Transform target;

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

        transform.position =
            Vector3.Lerp(
                transform.position,
                desiredPosition,
                followSpeed * Time.deltaTime);
    }
}