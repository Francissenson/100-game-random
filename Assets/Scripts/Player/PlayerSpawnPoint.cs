using UnityEngine;

public sealed class PlayerSpawnPoint : MonoBehaviour
{
    private void Start()
    {
        GameObject player =
            PlayerSpawner.Instance.GetPlayer();

        if (player == null)
        {
            return;
        }

        player.transform.position =
            transform.position;
    }
}