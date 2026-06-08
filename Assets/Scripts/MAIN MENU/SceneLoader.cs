using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField]
    private TransitionCanvas transitionCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(
            LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(
        string sceneName)
    {
        if (transitionCanvas != null)
        {
            yield return StartCoroutine(
                transitionCanvas.CloseTransition());
        }

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(
                sceneName);

        operation.allowSceneActivation = false;

        float timer = 0f;

        while (operation.progress < 0.9f ||
               timer < transitionCanvas.MinimumLoadTime)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }

        yield return null;

        if (transitionCanvas != null)
        {
            yield return StartCoroutine(
                transitionCanvas.OpenTransition());
        }
    }
}