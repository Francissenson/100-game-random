using UnityEngine;

public sealed class MainMenuUI : MonoBehaviour
{
    public void StartRun()
    {
        SceneLoader.Instance.LoadScene(
            "StartingRoom");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}