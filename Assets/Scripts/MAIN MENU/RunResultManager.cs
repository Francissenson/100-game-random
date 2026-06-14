using UnityEngine;

public sealed class RunResultManager : MonoBehaviour
{
    public static RunResultManager Instance
    {
        get;
        private set;
    }

    public bool Victory
    {
        get;
        private set;
    }

    public bool RunFinished
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void SetVictory()
    {
        if (RunFinished)
        {
            return;
        }

        Victory = true;
        RunFinished = true;

        AudioManager.Instance?.PlayVictorySting();
    }

    public void SetGameOver()
    {
        if (RunFinished)
        {
            return;
        }

        Victory = false;
        RunFinished = true;

        AudioManager.Instance?.PlayDefeatSting();
    }

    public void ResetResult()
    {
        Victory = false;
        RunFinished = false;
    }
}
