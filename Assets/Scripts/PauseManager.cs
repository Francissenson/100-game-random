using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class PauseManager : MonoBehaviour
{
    private static PauseManager instance;

    public static PauseManager Instance => instance;

    public static bool IsPaused => instance != null && instance.isPaused;

    private GameObject canvasObject;
    private CanvasGroup overlayGroup;
    private float defaultFixedDeltaTime;
    private bool isPaused;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null)
        {
            return;
        }

        GameObject pauseObject = new GameObject("PauseManager");
        instance = pauseObject.AddComponent<PauseManager>();
        DontDestroyOnLoad(pauseObject);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        defaultFixedDeltaTime = Time.fixedDeltaTime;
        BuildOverlay();
        SetPaused(false, true);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SetPaused(false, true);
            instance = null;
        }
    }

    private void Update()
    {
        if (!IsGameplayScene())
        {
            if (isPaused)
            {
                SetPaused(false);
            }

            return;
        }

        if (AudioManager.IsSceneLoading ||
            TransitionCanvas.IsTransitioning)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetPaused(false, true);
    }

    public void TogglePause()
    {
        SetPaused(!isPaused);
    }

    public void SetPaused(
        bool paused,
        bool force = false)
    {
        if (!force &&
            isPaused == paused)
        {
            return;
        }

        isPaused = paused;

        Time.timeScale = paused ? 0f : 1f;
        Time.fixedDeltaTime = defaultFixedDeltaTime;
        AudioListener.pause = paused;

        if (overlayGroup != null)
        {
            overlayGroup.alpha = paused ? 1f : 0f;
            overlayGroup.interactable = paused;
            overlayGroup.blocksRaycasts = paused;
            canvasObject.SetActive(paused);
        }
    }

    private bool IsGameplayScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        return sceneName.StartsWith("CombatRoom") ||
               sceneName == "BossRoom";
    }

    private void BuildOverlay()
    {
        canvasObject = new GameObject(
            "PauseCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));

        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject overlay = new GameObject(
            "PauseOverlay",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(CanvasGroup));

        overlay.transform.SetParent(canvasObject.transform, false);

        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        Image overlayImage = overlay.GetComponent<Image>();
        overlayImage.color = new Color(0.35f, 0f, 0f, 0.68f);
        overlayImage.raycastTarget = true;

        overlayGroup = overlay.GetComponent<CanvasGroup>();
        overlayGroup.alpha = 0f;
        overlayGroup.interactable = false;
        overlayGroup.blocksRaycasts = false;

        CreateLabel(
            overlay.transform,
            "PausedTitle",
            "PAUSED",
            74,
            new Vector2(0.5f, 0.58f),
            new Vector2(520f, 120f),
            new Color(1f, 0.94f, 0.78f, 1f));

        CreateLabel(
            overlay.transform,
            "PausedHint",
            "Press ESC to resume",
            30,
            new Vector2(0.5f, 0.47f),
            new Vector2(440f, 60f),
            new Color(1f, 1f, 1f, 0.92f));

        canvasObject.SetActive(false);
    }

    private TMP_Text CreateLabel(
        Transform parent,
        string objectName,
        string text,
        int fontSize,
        Vector2 anchor,
        Vector2 size,
        Color color)
    {
        GameObject textObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));

        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;

        TMP_Text label = textObject.GetComponent<TMP_Text>();
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.color = color;
        label.enableWordWrapping = false;
        label.raycastTarget = false;

        return label;
    }
}
