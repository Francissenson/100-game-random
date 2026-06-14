using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip bossMusic;

    [Header("Stingers")]
    [SerializeField] private AudioClip victorySting;
    [SerializeField] private AudioClip defeatSting;

    [Header("UI")]
    [SerializeField] private AudioClip buttonClickSfx;

    [Header("Player")]
    [SerializeField] private AudioClip gunshotSfx;
    [SerializeField] private AudioClip swordSlashSfx;
    [SerializeField] private AudioClip playerDamageSfx;
    [SerializeField] private AudioClip dashSfx;
    [SerializeField] private AudioClip weaponSwapSfx;

    [Header("Enemy")]
    [SerializeField] private AudioClip enemyHitSfx;
    [SerializeField] private Vector2 enemyHitPitchRange =
        new Vector2(0.9f, 1.15f);
    [SerializeField] private AudioClip enemyDeathSfx;
    [SerializeField] private AudioClip tankAlarmSfx;
    [SerializeField] private AudioClip bossAlarmSfx;
    [SerializeField] private AudioClip enemyShootSfx;

    [Header("Rewards")]
    [SerializeField] private AudioClip upgradeSfx;
    [SerializeField] private AudioClip weaponPickupSfx;

    [Header("Volume")]
    [SerializeField] private float musicVolume = 1f;
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float loadingVolumeMultiplier = 0.35f;
    [SerializeField] private float loadingVolumeFadeDuration = 0.15f;

    private AudioSource musicSource;
    private AudioClip currentMusic;
    private float currentVolumeMultiplier = 1f;
    private int loadingDuckRequests;
    private Coroutine loadingDuckRoutine;
    private static int sceneLoadCount;

    public static bool IsSceneLoading => sceneLoadCount > 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
        {
            return;
        }

        AudioManager existingAudioManager =
            Object.FindFirstObjectByType<AudioManager>();

        if (existingAudioManager != null)
        {
            Instance = existingAudioManager;
            DontDestroyOnLoad(
                existingAudioManager.gameObject);
            return;
        }

        GameObject audioObject =
            new GameObject("AudioManager");

        Instance =
            audioObject.AddComponent<AudioManager>();

        DontDestroyOnLoad(
            audioObject);
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

        musicSource =
            gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.spatialBlend = 0f;

        SceneManager.sceneLoaded +=
            OnSceneLoaded;

        ApplySceneMusic(
            SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -=
                OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        ApplySceneMusic(
            scene.name);
    }

    private void ApplySceneMusic(
        string sceneName)
    {
        if (string.Equals(
                sceneName,
                "MainMenu"))
        {
            PlayMusic(
                mainMenuMusic);
            return;
        }

        if (string.Equals(
                sceneName,
                "EndRunScene"))
        {
            StopMusic();
            return;
        }

        if (sceneName.StartsWith("StartingRoom") ||
            sceneName.StartsWith("CombatRoom") ||
            sceneName.StartsWith("TreasureRoom") ||
            sceneName.StartsWith("ExitRoom") ||
            string.Equals(sceneName, "BossRoom"))
        {
            PlayMusic(
                gameplayMusic);
        }
    }

    public void PlayMusic(
        AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        if (currentMusic == clip &&
            musicSource != null &&
            musicSource.isPlaying)
        {
            return;
        }

        currentMusic = clip;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume * currentVolumeMultiplier;
        musicSource.pitch = 1f;
        musicSource.Play();
    }

    public void PlayBossMusic()
    {
        PlayMusic(
            bossMusic);
    }

    public void StopMusic()
    {
        currentMusic = null;

        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.clip = null;
        }
    }

    public void PlayButtonClick()
    {
        PlaySfx(
            buttonClickSfx);
    }

    public void PlayGunshot()
    {
        PlaySfx(
            gunshotSfx);
    }

    public void PlaySwordSlash()
    {
        PlaySfx(
            swordSlashSfx);
    }

    public void PlayPlayerDamage()
    {
        PlaySfx(
            playerDamageSfx);
    }

    public void PlayDash()
    {
        PlaySfx(
            dashSfx);
    }

    public void PlayWeaponSwap()
    {
        PlaySfx(
            weaponSwapSfx);
    }

    public void PlayEnemyHit()
    {
        PlaySfx(
            enemyHitSfx,
            Random.Range(
                enemyHitPitchRange.x,
                enemyHitPitchRange.y));
    }

    public void PlayTankAlarm()
    {
        PlaySfx(
            tankAlarmSfx);
    }

    public void PlayBossAlarm()
    {
        PlaySfx(
            bossAlarmSfx);
    }

    public void PlayEnemyShoot()
    {
        PlaySfx(
            enemyShootSfx);
    }

    public void PlayUpgradeSfx()
    {
        PlaySfx(
            upgradeSfx);
    }

    public void PlayWeaponPickupSfx()
    {
        PlaySfx(
            weaponPickupSfx);
    }

    public void BeginSceneLoadDucking(
        bool instant = false)
    {
        loadingDuckRequests++;
        sceneLoadCount++;

        if (loadingDuckRequests == 1)
        {
            if (instant)
            {
                if (loadingDuckRoutine != null)
                {
                    StopCoroutine(
                        loadingDuckRoutine);
                    loadingDuckRoutine = null;
                }

                ApplyVolumeMultiplier(
                    loadingVolumeMultiplier);
                return;
            }

            StartLoadingVolumeFade(
                loadingVolumeMultiplier);
        }
    }

    public void EndSceneLoadDucking(
        bool instant = false)
    {
        if (loadingDuckRequests > 0)
        {
            loadingDuckRequests--;
        }

        if (sceneLoadCount > 0)
        {
            sceneLoadCount--;
        }

        if (loadingDuckRequests == 0)
        {
            if (instant)
            {
                if (loadingDuckRoutine != null)
                {
                    StopCoroutine(
                        loadingDuckRoutine);
                    loadingDuckRoutine = null;
                }

                ApplyVolumeMultiplier(1f);
                return;
            }

            StartLoadingVolumeFade(
                1f);
        }
    }

    public void PlayEnemyDeath()
    {
        PlaySfx(
            enemyDeathSfx,
            Random.Range(
                enemyHitPitchRange.x,
                enemyHitPitchRange.y));
    }

    public void PlayVictorySting()
    {
        PlayStinger(
            victorySting);
    }

    public void PlayDefeatSting()
    {
        PlayStinger(
            defeatSting);
    }

    private void PlayStinger(
        AudioClip clip)
    {
        StopMusic();
        PlaySfx(
            clip);
    }

    private void PlaySfx(
        AudioClip clip,
        float pitch = 1f)
    {
        if (clip == null)
        {
            return;
        }

        GameObject tempObject =
            new GameObject(
                $"{clip.name}_Sfx");

        tempObject.transform.SetParent(
            transform,
            false);

        AudioSource source =
            tempObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.playOnAwake = false;
        source.loop = false;
        source.volume =
            sfxVolume * currentVolumeMultiplier;
        source.pitch = Mathf.Max(0.1f, pitch);
        source.spatialBlend = 0f;
        source.Play();

        float lifetime =
            clip.length /
            Mathf.Max(0.1f, source.pitch);

        Destroy(
            tempObject,
            lifetime + 0.1f);
    }

    private void StartLoadingVolumeFade(
        float targetMultiplier)
    {
        if (loadingDuckRoutine != null)
        {
            StopCoroutine(
                loadingDuckRoutine);
        }

        loadingDuckRoutine =
            StartCoroutine(
                LoadingVolumeFadeRoutine(
                    targetMultiplier));
    }

    private IEnumerator LoadingVolumeFadeRoutine(
        float targetMultiplier)
    {
        float startMultiplier =
            currentVolumeMultiplier;
        float timer = 0f;

        while (timer < loadingVolumeFadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    timer / loadingVolumeFadeDuration);

            ApplyVolumeMultiplier(
                Mathf.Lerp(
                    startMultiplier,
                    targetMultiplier,
                    t));

            yield return null;
        }

        ApplyVolumeMultiplier(
            targetMultiplier);

        loadingDuckRoutine = null;
    }

    private void ApplyVolumeMultiplier(
        float multiplier)
    {
        currentVolumeMultiplier =
            Mathf.Clamp01(multiplier);

        if (musicSource != null)
        {
            musicSource.volume =
                musicVolume *
                currentVolumeMultiplier;
        }
    }
}
