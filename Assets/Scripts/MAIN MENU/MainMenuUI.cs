using UnityEngine;

public sealed class MainMenuUI : MonoBehaviour
{
    public void StartRun()
    {
        if (RunStatsManager.Instance != null)
        {
            RunStatsManager.Instance.ResetRun();
        }

        if (RunResultManager.Instance != null)
        {
            RunResultManager.Instance.ResetResult();
        }

        PlayerSpawner.Instance.SpawnPlayer();

        SceneLoader.Instance.LoadScene(
            "StartingRoom");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
