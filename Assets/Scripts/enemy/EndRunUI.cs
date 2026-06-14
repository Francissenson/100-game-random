using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public sealed class EndRunUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject summaryPanel;

    [SerializeField] private CanvasGroup blackOverlay;

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
        AutoWire();
        ShowResults();
    }

    public void ShowResults()
    {
        StartCoroutine(ShowResultsRoutine());
    }

    private IEnumerator ShowResultsRoutine()
    {
        if (resultPanel == null ||
            summaryPanel == null)
        {
            Debug.LogError(
                "[EndRunUI] Result or summary panel missing.");

            yield break;
        }

        if (blackOverlay != null)
        {
            blackOverlay.alpha = 1f;
            blackOverlay.gameObject.SetActive(true);
        }

        PopulateSummary();

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        summaryPanel.SetActive(false);

        yield return new WaitForSeconds(0.6f);

        if (blackOverlay != null)
        {
            yield return StartCoroutine(FadeOverlay(blackOverlay, 1f, 0f, 0.35f));
        }

        resultPanel.SetActive(true);

        yield return new WaitForSeconds(resultDuration);

        summaryPanel.SetActive(true);
    }

    private IEnumerator FadeOverlay(
        CanvasGroup overlay,
        float from,
        float to,
        float duration)
    {
        if (overlay == null)
        {
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            overlay.alpha =
                Mathf.Lerp(from, to, timer / duration);

            yield return null;
        }

        overlay.alpha = to;

        if (Mathf.Approximately(to, 0f))
        {
            overlay.gameObject.SetActive(false);
        }
    }

    private void PopulateSummary()
    {
        if (RunStatsManager.Instance == null)
        {
            Debug.LogWarning(
                "[EndRunUI] RunStatsManager missing. Summary values will be empty.");

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

    private void AutoWire()
    {
        if (resultPanel == null)
        {
            resultPanel =
                FindNamedObject("ResultPanel");
        }

        if (summaryPanel == null)
        {
            summaryPanel =
                FindNamedObject("RunSummaryPanel");
        }

        if (blackOverlay == null)
        {
            blackOverlay =
                FindBlackOverlay();

            if (blackOverlay == null)
            {
                blackOverlay =
                    CreateBlackOverlay();
            }
        }

        if (resultText == null)
        {
            resultText = FindText("Result");
        }

        if (goldText == null)
        {
            goldText = FindText("Gold");
        }

        if (killsText == null)
        {
            killsText = FindText("Kills");
        }

        if (roomsText == null)
        {
            roomsText = FindText("Rooms");
        }

        if (timeText == null)
        {
            timeText = FindText("Time");
        }
    }

    private GameObject FindNamedObject(string objectName)
    {
        GameObject[] roots =
            gameObject.scene.GetRootGameObjects();

        foreach (GameObject root in roots)
        {
            Transform match =
                FindDeepChild(root.transform, objectName);

            if (match != null)
            {
                return match.gameObject;
            }
        }

        return null;
    }

    private CanvasGroup FindBlackOverlay()
    {
        CanvasGroup[] overlays =
            FindObjectsByType<CanvasGroup>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

        foreach (CanvasGroup overlay in overlays)
        {
            if (overlay == null)
            {
                continue;
            }

            if (overlay.gameObject == gameObject)
            {
                continue;
            }

            if (overlay.gameObject.name.ToLower().Contains("black") ||
                overlay.gameObject.name.ToLower().Contains("overlay"))
            {
                return overlay;
            }
        }

        return null;
    }

    private CanvasGroup CreateBlackOverlay()
    {
        Canvas rootCanvas =
            GetComponentInParent<Canvas>();

        Transform parent =
            rootCanvas != null
                ? rootCanvas.transform
                : transform;

        GameObject overlayObject =
            new GameObject(
                "BlackOverlay",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(CanvasGroup));

        overlayObject.transform.SetParent(
            parent,
            false);

        RectTransform rect =
            overlayObject.GetComponent<RectTransform>();

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;

        Image image =
            overlayObject.GetComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = true;

        CanvasGroup group =
            overlayObject.GetComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        overlayObject.transform.SetAsFirstSibling();

        Debug.Log(
            "[EndRunUI] Created fallback black overlay.");

        return group;
    }

    private TMP_Text FindText(string labelHint)
    {
        TMP_Text[] texts =
            FindObjectsByType<TMP_Text>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

        foreach (TMP_Text text in texts)
        {
            if (text == null)
            {
                continue;
            }

            if (text.gameObject == gameObject)
            {
                continue;
            }

            string objectName = text.gameObject.name.ToLower();

            if (objectName.Contains(labelHint.ToLower()))
            {
                return text;
            }
        }

        return null;
    }

    private Transform FindDeepChild(
        Transform parent,
        string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName ||
                child.name.Trim() == childName)
            {
                return child;
            }

            Transform result =
                FindDeepChild(
                    child,
                    childName);

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    public void ReturnToMenu()
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

        AudioManager.Instance?.BeginSceneLoadDucking(true);
        SceneManager.LoadScene("MainMenu");
        AudioManager.Instance?.EndSceneLoadDucking(true);
    }
}
