using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameHUD : MonoBehaviour
{
    private const int SlotCount = 3;

    private static GameHUD instance;

    public static GameHUD Instance => instance;

    private PlayerHealth playerHealth;
    private WeaponManager weaponManager;
    private readonly List<WaveManager> waveManagers =
        new List<WaveManager>();

    private GameObject canvasObject;
    private RectTransform healthFillRect;
    private TMP_Text healthText;
    private TMP_Text goldText;
    private TMP_Text wavePromptText;
    private CanvasGroup roomClearFlashGroup;
    private SlotView[] slots;
    private float wavePromptCountdownEndTime;
    private float wavePromptVisibleUntil;
    private int wavePromptNumber;
    private Coroutine roomClearFlashRoutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null)
        {
            return;
        }

        GameObject hudObject = new GameObject("GameHUD");
        instance = hudObject.AddComponent<GameHUD>();
        DontDestroyOnLoad(hudObject);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        BuildHud();
        SceneManager.sceneLoaded += OnSceneLoaded;
        RefreshTargets();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            UnsubscribeWaveManagers();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HideWavePrompt();
        RefreshTargets();
    }

    private void Update()
    {
        bool shouldShow =
            ShouldShowHud();

        if (!shouldShow)
        {
            RefreshTargets();
            shouldShow =
                ShouldShowHud();
        }

        canvasObject.SetActive(shouldShow);

        if (!shouldShow)
        {
            return;
        }

        RefreshHealth();
        RefreshGold();
        RefreshWeaponSlots();
        RefreshWavePrompt();
    }

    private bool ShouldShowHud()
    {
        if (!IsGameplayScene())
        {
            return false;
        }

        if (TransitionCanvas.IsTransitioning)
        {
            return false;
        }

        if (PauseManager.IsPaused)
        {
            return false;
        }

        if (RunResultManager.Instance != null &&
            RunResultManager.Instance.RunFinished)
        {
            return false;
        }

        if (playerHealth != null &&
            playerHealth.IsDead)
        {
            return false;
        }

        return playerHealth != null || weaponManager != null;
    }

    private bool IsGameplayScene()
    {
        string sceneName =
            SceneManager.GetActiveScene().name;

        return sceneName.StartsWith("CombatRoom") ||
               sceneName.StartsWith("TreasureRoom") ||
               sceneName == "BossRoom";
    }

    private void RefreshTargets()
    {
        playerHealth =
            FindFirstObjectByType<PlayerHealth>();

        weaponManager =
            FindFirstObjectByType<WeaponManager>();

        SetWaveManagers(
            FindObjectsByType<WaveManager>(
                FindObjectsSortMode.None));
    }

    private void SetWaveManagers(
        WaveManager[] sceneWaveManagers)
    {
        UnsubscribeWaveManagers();

        foreach (WaveManager waveManager in sceneWaveManagers)
        {
            if (waveManager == null)
            {
                continue;
            }

            waveManager.OnWaveStartingSoon +=
                HandleWaveStartingSoon;

            waveManager.OnWaveStarted +=
                HandleWaveStarted;

            waveManagers.Add(
                waveManager);
        }
    }

    private void UnsubscribeWaveManagers()
    {
        foreach (WaveManager waveManager in waveManagers)
        {
            if (waveManager == null)
            {
                continue;
            }

            waveManager.OnWaveStartingSoon -=
                HandleWaveStartingSoon;

            waveManager.OnWaveStarted -=
                HandleWaveStarted;
        }

        waveManagers.Clear();
    }

    private void HandleWaveStartingSoon(
        int waveNumber,
        float delay)
    {
        wavePromptNumber =
            waveNumber;

        float duration =
            Mathf.Max(
                delay,
                0f);

        wavePromptCountdownEndTime =
            Time.time + duration;

        wavePromptVisibleUntil =
            wavePromptCountdownEndTime;

        wavePromptText.gameObject.SetActive(
            true);
    }

    private void HandleWaveStarted(
        int waveNumber)
    {
        wavePromptNumber =
            waveNumber;

        wavePromptVisibleUntil =
            Time.time + 1.2f;

        wavePromptText.text =
            $"Wave {wavePromptNumber}!";

        wavePromptText.gameObject.SetActive(
            true);
    }

    private void RefreshWavePrompt()
    {
        if (wavePromptText == null)
        {
            return;
        }

        if (Time.time >= wavePromptVisibleUntil)
        {
            wavePromptText.gameObject.SetActive(
                false);

            return;
        }

        if (Time.time < wavePromptCountdownEndTime)
        {
            int secondsRemaining =
                Mathf.CeilToInt(
                    wavePromptCountdownEndTime - Time.time);

            wavePromptText.text =
                $"Wave {wavePromptNumber} starts in {secondsRemaining}";
        }
    }

    private void HideWavePrompt()
    {
        if (wavePromptText == null)
        {
            return;
        }

        wavePromptText.gameObject.SetActive(
            false);

        wavePromptText.text =
            string.Empty;

        wavePromptCountdownEndTime =
            0f;

        wavePromptVisibleUntil =
            0f;
    }

    public void PlayRoomClearFlash()
    {
        if (roomClearFlashGroup == null)
        {
            return;
        }

        if (roomClearFlashRoutine != null)
        {
            StopCoroutine(
                roomClearFlashRoutine);
        }

        roomClearFlashRoutine =
            StartCoroutine(
                RoomClearFlashRoutine());
    }

    private void RefreshHealth()
    {
        if (playerHealth == null)
        {
            healthFillRect.anchorMax = new Vector2(0f, 1f);
            healthText.text = "HP 0 / 0";
            return;
        }

        int current =
            playerHealth.CurrentHealth;

        int max =
            Mathf.Max(1, playerHealth.MaxHealth);

        float normalized =
            Mathf.Clamp01((float)current / max);

        healthFillRect.anchorMax =
            new Vector2(normalized, 1f);
        healthFillRect.offsetMax =
            Vector2.zero;

        healthText.text =
            $"HP {current} / {max}";
    }

    private void RefreshGold()
    {
        int gold =
            RunStatsManager.Instance != null
                ? RunStatsManager.Instance.GoldEarned
                : 0;

        goldText.text =
            $"Gold: {gold}";
    }

    private void RefreshWeaponSlots()
    {
        if (weaponManager == null)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].SetWeapon(i + 1, null, false);
            }

            return;
        }

        for (int i = 0; i < SlotCount; i++)
        {
            int slotNumber = i + 1;
            Weapon weapon =
                weaponManager.GetWeaponInSlot(slotNumber);

            slots[i].SetWeapon(
                slotNumber,
                weapon,
                weaponManager.CurrentSlot == slotNumber);
        }
    }

    private void BuildHud()
    {
        canvasObject =
            new GameObject(
                "GameplayHUDCanvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));

        canvasObject.transform.SetParent(transform, false);

        Canvas canvas =
            canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler =
            canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        BuildHealthPanel(canvasObject.transform);
        BuildGoldPanel(canvasObject.transform);
        BuildWeaponSlots(canvasObject.transform);
        BuildWavePrompt(canvasObject.transform);
        BuildRoomClearFlash(canvasObject.transform);
    }

    private void BuildHealthPanel(Transform parent)
    {
        RectTransform panel =
            CreatePanel(parent, "HealthPanel", new Color(0.05f, 0.06f, 0.07f, 0.82f));

        Anchor(panel, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(32f, -32f));
        panel.sizeDelta = new Vector2(360f, 64f);

        RectTransform back =
            CreateImage(panel, "HealthBack", new Color(0.16f, 0.04f, 0.05f, 1f));

        Anchor(back, Vector2.zero, Vector2.one, Vector2.zero);
        back.offsetMin = new Vector2(16f, 14f);
        back.offsetMax = new Vector2(-16f, -14f);

        Image healthFill =
            CreateImage(back, "HealthFill", new Color(0.86f, 0.08f, 0.1f, 1f))
                .GetComponent<Image>();

        healthFillRect =
            healthFill.rectTransform;
        Anchor(healthFillRect, Vector2.zero, Vector2.one, Vector2.zero);
        healthFillRect.pivot = new Vector2(0f, 0.5f);
        healthFillRect.offsetMin = Vector2.zero;
        healthFillRect.offsetMax = Vector2.zero;

        healthText =
            CreateText(panel, "HealthText", "HP 100 / 100", 25, TextAlignmentOptions.Center);

        RectTransform textRect =
            healthText.rectTransform;
        Anchor(textRect, Vector2.zero, Vector2.one, Vector2.zero);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }

    private void BuildGoldPanel(Transform parent)
    {
        RectTransform panel =
            CreatePanel(parent, "GoldPanel", new Color(0.05f, 0.06f, 0.07f, 0.82f));

        Anchor(panel, Vector2.one, Vector2.one, new Vector2(-32f, -32f));
        panel.sizeDelta = new Vector2(220f, 58f);

        goldText =
            CreateText(panel, "GoldText", "Gold: 0", 27, TextAlignmentOptions.Center);

        RectTransform textRect =
            goldText.rectTransform;
        Anchor(textRect, Vector2.zero, Vector2.one, Vector2.zero);
        textRect.offsetMin = new Vector2(12f, 0f);
        textRect.offsetMax = new Vector2(-12f, 0f);
        goldText.color = new Color(1f, 0.82f, 0.18f, 1f);
    }

    private void BuildWeaponSlots(Transform parent)
    {
        RectTransform container =
            new GameObject("WeaponSlots", typeof(RectTransform)).GetComponent<RectTransform>();

        container.SetParent(parent, false);
        Anchor(container, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 36f));
        container.sizeDelta = new Vector2(420f, 96f);

        HorizontalLayoutGroup layout =
            container.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 14f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        slots = new SlotView[SlotCount];

        for (int i = 0; i < SlotCount; i++)
        {
            slots[i] =
                SlotView.Create(container);
        }
    }

    private void BuildWavePrompt(Transform parent)
    {
        wavePromptText =
            CreateText(
                parent,
                "WavePromptText",
                string.Empty,
                48,
                TextAlignmentOptions.Center);

        RectTransform textRect =
            wavePromptText.rectTransform;

        Anchor(
            textRect,
            new Vector2(0.5f, 0.78f),
            new Vector2(0.5f, 0.78f),
            Vector2.zero);

        textRect.sizeDelta =
            new Vector2(760f, 90f);

        wavePromptText.color =
            new Color(1f, 0.86f, 0.28f, 1f);

        Outline outline =
            wavePromptText.gameObject.AddComponent<Outline>();
        outline.effectColor =
            new Color(0f, 0f, 0f, 0.9f);
        outline.effectDistance =
            new Vector2(2f, -2f);

        wavePromptText.gameObject.SetActive(
            false);
    }

    private void BuildRoomClearFlash(Transform parent)
    {
        GameObject flashObject =
            new GameObject(
                "RoomClearFlash",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(CanvasGroup));

        flashObject.transform.SetParent(
            parent,
            false);
        flashObject.transform.SetAsLastSibling();

        RectTransform rect =
            flashObject.GetComponent<RectTransform>();

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;

        Image image =
            flashObject.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0f);
        image.raycastTarget = false;

        roomClearFlashGroup =
            flashObject.GetComponent<CanvasGroup>();
        roomClearFlashGroup.alpha = 0f;
        roomClearFlashGroup.interactable = false;
        roomClearFlashGroup.blocksRaycasts = false;
    }

    private IEnumerator RoomClearFlashRoutine()
    {
        const float fadeInDuration = 0.5f;
        const float fadeOutDuration = 0.5f;
        const float peakAlpha = 0.88f;

        float timer = 0f;

        while (timer < fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;

            roomClearFlashGroup.alpha =
                Mathf.Lerp(0f, peakAlpha, timer / fadeInDuration);

            yield return null;
        }

        timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime;

            roomClearFlashGroup.alpha =
                Mathf.Lerp(peakAlpha, 0f, timer / fadeOutDuration);

            yield return null;
        }

        roomClearFlashGroup.alpha = 0f;
        roomClearFlashRoutine = null;
    }

    private RectTransform CreatePanel(Transform parent, string objectName, Color color)
    {
        RectTransform rect =
            CreateImage(parent, objectName, color);

        Outline outline =
            rect.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.7f);
        outline.effectDistance = new Vector2(2f, -2f);

        return rect;
    }

    private RectTransform CreateImage(Transform parent, string objectName, Color color)
    {
        GameObject imageObject =
            new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));

        imageObject.transform.SetParent(parent, false);

        Image image =
            imageObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        return image.rectTransform;
    }

    private TMP_Text CreateText(
        Transform parent,
        string objectName,
        string text,
        int fontSize,
        TextAlignmentOptions alignment)
    {
        GameObject textObject =
            new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI));

        textObject.transform.SetParent(parent, false);

        TMP_Text label =
            textObject.GetComponent<TMP_Text>();
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = alignment;
        label.color = Color.white;
        label.enableWordWrapping = false;
        label.raycastTarget = false;

        return label;
    }

    private static void Anchor(
        RectTransform rect,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPosition)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(
            Mathf.Lerp(anchorMin.x, anchorMax.x, 0.5f),
            Mathf.Lerp(anchorMin.y, anchorMax.y, 0.5f));
        rect.anchoredPosition = anchoredPosition;
    }

    private sealed class SlotView
    {
        private readonly Image background;
        private readonly Image icon;
        private readonly TMP_Text numberText;
        private readonly TMP_Text nameText;

        private SlotView(
            Image background,
            Image icon,
            TMP_Text numberText,
            TMP_Text nameText)
        {
            this.background = background;
            this.icon = icon;
            this.numberText = numberText;
            this.nameText = nameText;
        }

        public static SlotView Create(Transform parent)
        {
            GameObject slotObject =
                new GameObject(
                    "WeaponSlot",
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(Image),
                    typeof(Outline));

            slotObject.transform.SetParent(parent, false);

            RectTransform rect =
                slotObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(124f, 92f);

            Image background =
                slotObject.GetComponent<Image>();
            background.color = new Color(0.05f, 0.06f, 0.07f, 0.82f);
            background.raycastTarget = false;

            Outline outline =
                slotObject.GetComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.7f);
            outline.effectDistance = new Vector2(2f, -2f);

            Image icon =
                CreateSlotImage(rect, "Icon", new Color(1f, 1f, 1f, 0.92f));
            RectTransform iconRect = icon.rectTransform;
            Anchor(iconRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 10f));
            iconRect.sizeDelta = new Vector2(42f, 42f);
            icon.preserveAspect = true;

            TMP_Text number =
                CreateSlotText(rect, "SlotNumber", "1", 18, TextAlignmentOptions.TopLeft);
            RectTransform numberRect = number.rectTransform;
            Anchor(numberRect, Vector2.zero, Vector2.one, Vector2.zero);
            numberRect.offsetMin = new Vector2(8f, 6f);
            numberRect.offsetMax = new Vector2(-8f, -6f);

            TMP_Text name =
                CreateSlotText(rect, "WeaponName", "Empty", 16, TextAlignmentOptions.Bottom);
            RectTransform nameRect = name.rectTransform;
            Anchor(nameRect, Vector2.zero, Vector2.one, Vector2.zero);
            nameRect.offsetMin = new Vector2(8f, 5f);
            nameRect.offsetMax = new Vector2(-8f, -58f);

            return new SlotView(background, icon, number, name);
        }

        public void SetWeapon(int slotNumber, Weapon weapon, bool selected)
        {
            numberText.text = slotNumber.ToString();
            nameText.text = weapon != null ? weapon.gameObject.name : "Empty";

            Sprite weaponSprite =
                weapon != null
                    ? weapon.GetComponentInChildren<SpriteRenderer>(true)?.sprite
                    : null;

            icon.sprite = weaponSprite;
            icon.enabled = weaponSprite != null;

            background.color =
                selected
                    ? new Color(0.96f, 0.72f, 0.18f, 0.92f)
                    : new Color(0.05f, 0.06f, 0.07f, 0.82f);
        }

        private static Image CreateSlotImage(Transform parent, string objectName, Color color)
        {
            GameObject imageObject =
                new GameObject(
                    objectName,
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(Image));

            imageObject.transform.SetParent(parent, false);

            Image image =
                imageObject.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;

            return image;
        }

        private static TMP_Text CreateSlotText(
            Transform parent,
            string objectName,
            string text,
            int fontSize,
            TextAlignmentOptions alignment)
        {
            GameObject textObject =
                new GameObject(
                    objectName,
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(TextMeshProUGUI));

            textObject.transform.SetParent(parent, false);

            TMP_Text label =
                textObject.GetComponent<TMP_Text>();
            label.text = text;
            label.fontSize = fontSize;
            label.alignment = alignment;
            label.color = Color.white;
            label.enableWordWrapping = false;
            label.raycastTarget = false;

            return label;
        }
    }
}
