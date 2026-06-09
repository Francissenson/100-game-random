using UnityEngine;

public sealed class MainMenuUI : MonoBehaviour
{
    public void StartRun()
    {
        PlayerSpawner.Instance.SpawnPlayer();

        SceneLoader.Instance.LoadScene(
            "StartingRoom");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}