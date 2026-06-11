using System.Collections;
using UnityEngine;

public sealed class TransitionCanvas : MonoBehaviour
{
    [Header("Doors")]
    [SerializeField] private RectTransform leftDoor;

    [SerializeField] private RectTransform rightDoor;

    [SerializeField] private float slideDuration = 0.5f;

    [Header("Loading")]
    [SerializeField] private GameObject loadingArtwork;

    [SerializeField] private float minimumLoadTime = 2f;

    public float MinimumLoadTime =>
        minimumLoadTime;

    private Vector2 leftClosed;

    private Vector2 rightClosed;

    private Vector2 leftOpen;

    private Vector2 rightOpen;

    private bool isTransitioning;

    public static bool IsTransitioning { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        InitializeDoorPositions();

        if (loadingArtwork != null)
        {
            loadingArtwork.SetActive(false);
        }

        if (!isTransitioning)
        {
            gameObject.SetActive(false);
        }
    }

    private void InitializeDoorPositions()
    {
        leftClosed =
            new Vector2(
                -480f,
                0f);

        rightClosed =
            new Vector2(
                480f,
                0f);

        leftOpen =
            new Vector2(
                -1440f,
                0f);

        rightOpen =
            new Vector2(
                1440f,
                0f);

        leftDoor.anchoredPosition =
            leftOpen;

        rightDoor.anchoredPosition =
            rightOpen;
    }

    public IEnumerator CloseTransition()
    {
        isTransitioning = true;
        IsTransitioning = true;

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        float timer = 0f;

        while (timer < slideDuration)
        {
            timer += Time.deltaTime;

            float t =
                timer / slideDuration;

            leftDoor.anchoredPosition =
                Vector2.Lerp(
                    leftOpen,
                    leftClosed,
                    t);

            rightDoor.anchoredPosition =
                Vector2.Lerp(
                    rightOpen,
                    rightClosed,
                    t);

            yield return null;
        }

        leftDoor.anchoredPosition =
            leftClosed;

        rightDoor.anchoredPosition =
            rightClosed;

        if (loadingArtwork != null)
        {
            loadingArtwork.SetActive(true);
        }
    }

    public IEnumerator OpenTransition()
    {
        if (loadingArtwork != null)
        {
            loadingArtwork.SetActive(false);
        }

        float timer = 0f;

        while (timer < slideDuration)
        {
            timer += Time.deltaTime;

            float t =
                timer / slideDuration;

            leftDoor.anchoredPosition =
                Vector2.Lerp(
                    leftClosed,
                    leftOpen,
                    t);

            rightDoor.anchoredPosition =
                Vector2.Lerp(
                    rightClosed,
                    rightOpen,
                    t);

            yield return null;
        }

        leftDoor.anchoredPosition =
            leftOpen;

        rightDoor.anchoredPosition =
            rightOpen;

        isTransitioning = false;
        IsTransitioning = false;

        gameObject.SetActive(false);
    }
}
