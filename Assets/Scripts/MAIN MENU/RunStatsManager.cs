using UnityEngine;

public sealed class RunStatsManager : MonoBehaviour
{
    public static RunStatsManager Instance
    {
        get;
        private set;
    }

    public int GoldEarned
    {
        get;
        private set;
    }

    public int EnemiesKilled
    {
        get;
        private set;
    }

    public int RoomsCleared
    {
        get;
        private set;
    }

    private float runStartTime;

    public float RunTime =>
        Time.time - runStartTime;

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

        runStartTime = Time.time;
    }

    public void AddGold(int amount)
    {
        GoldEarned += amount;
    }

    public void AddKill()
    {
        EnemiesKilled++;
    }

    public void AddRoomClear()
    {
        RoomsCleared++;
    }

    public void ResetRun()
    {
        GoldEarned = 0;
        EnemiesKilled = 0;
        RoomsCleared = 0;

        runStartTime = Time.time;
    }
}