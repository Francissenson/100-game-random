using UnityEngine;

public sealed class MainMenuUI : MonoBehaviour
{
    public void StartRun()
    {
        AudioManager.Instance?.PlayButtonClick();

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
        AudioManager.Instance?.PlayButtonClick();

        Application.Quit();
    }
}
