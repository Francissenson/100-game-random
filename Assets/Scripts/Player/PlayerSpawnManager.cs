using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class PlayerSpawnManager : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        PlayerSpawn spawn =
            FindFirstObjectByType<PlayerSpawn>();

        if (spawn == null)
        {
            return;
        }

        GameObject player =
            GameObject.FindGameObjectWithTag(
                "Player");

        if (player == null)
        {
            return;
        }

        player.transform.position =
            spawn.transform.position;
    }
}