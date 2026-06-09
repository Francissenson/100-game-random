using UnityEngine;

public sealed class ExitTriggerLoader : MonoBehaviour
{
    [SerializeField]
    private RoomExit roomExit;

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        roomExit.LoadNextRoom();
    }
}