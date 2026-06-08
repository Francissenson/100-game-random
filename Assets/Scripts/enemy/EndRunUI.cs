using System.Collections;
using TMPro;
using UnityEngine;

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

    [SerializeField] private float resultDuration = 3f;

    private IEnumerator Start()
    {
        resultPanel.SetActive(false);
        summaryPanel.SetActive(false);

        yield return null;

        StartCoroutine(
            ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        resultPanel.SetActive(true);

        if (RunResultManager.Instance.Victory)
        {
            resultText.text =
                "VICTORY";
        }
        else
        {
            resultText.text =
                "GAME OVER";
        }

        yield return new WaitForSeconds(
            resultDuration);

        resultPanel.SetActive(false);

        PopulateSummary();

        summaryPanel.SetActive(true);
    }

    private void PopulateSummary()
    {
        RunStatsManager stats =
            RunStatsManager.Instance;

        goldText.text =
            $"Gold: {stats.GoldEarned}";

        killsText.text =
            $"Kills: {stats.EnemiesKilled}";

        roomsText.text =
            $"Rooms: {stats.RoomsCleared}";

        int minutes =
            Mathf.FloorToInt(
                stats.RunTime / 60f);

        int seconds =
            Mathf.FloorToInt(
                stats.RunTime % 60f);

        timeText.text =
            $"Time: {minutes:00}:{seconds:00}";
    }

    public void ReturnToMenu()
    {
        RunStatsManager.Instance.ResetRun();

        SceneLoader.Instance.LoadScene(
            "MainMenu");
    }
}