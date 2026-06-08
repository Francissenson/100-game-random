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
        Victory = true;
    }

    public void SetGameOver()
    {
        Victory = false;
    }
}