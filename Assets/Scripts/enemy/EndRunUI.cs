using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class EndRunUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject summaryPanel;

    [Header("Texts")]
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text roomsText;
    [SerializeField] private TMP_Text timeText;

    [Header("Settings")]
    [SerializeField] private float resultDuration = 3f;

    private void Start()
    {
        ShowResults();
    }

    public void ShowResults()
    {
        StartCoroutine(ShowResultsRoutine());
    }

    private IEnumerator ShowResultsRoutine()
    {
        PopulateSummary();

        resultPanel.SetActive(true);

        summaryPanel.SetActive(false);

        yield return new WaitForSeconds(resultDuration);

        summaryPanel.SetActive(true);
    }

    private void PopulateSummary()
    {
        if (RunStatsManager.Instance == null)
        {
            return;
        }

        var stats = RunStatsManager.Instance;

        goldText.text =
            $"Gold: {stats.GoldEarned}";

        killsText.text =
            $"Kills: {stats.EnemiesKilled}";

        roomsText.text =
            $"Rooms: {stats.RoomsCleared}";

        int minutes =
            Mathf.FloorToInt(stats.RunTime / 60f);

        int seconds =
            Mathf.FloorToInt(stats.RunTime % 60f);

        timeText.text =
            $"Time: {minutes:00}:{seconds:00}";

        if (RunResultManager.Instance != null &&
            RunResultManager.Instance.Victory)
        {
            resultText.text = "VICTORY";
        }
        else
        {
            resultText.text = "GAME OVER";
        }
    }

    public void ReturnToMenu()
    {
        if (RunStatsManager.Instance != null)
        {
            RunStatsManager.Instance.ResetRun();
        }

        SceneManager.LoadScene("MainMenu");
    }
}