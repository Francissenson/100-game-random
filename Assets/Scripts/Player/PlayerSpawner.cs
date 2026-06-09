using UnityEngine;

public sealed class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance
    {
        get;
        private set;
    }

    [SerializeField]
    private GameObject playerPrefab;

    private GameObject currentPlayer;

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

    public GameObject SpawnPlayer()
    {
        if (currentPlayer != null)
        {
            return currentPlayer;
        }

        currentPlayer =
            Instantiate(playerPrefab);

        DontDestroyOnLoad(
            currentPlayer);

        return currentPlayer;
    }

    public void DestroyPlayer()
    {
        if (currentPlayer == null)
        {
            return;
        }

        Destroy(currentPlayer);

        currentPlayer = null;
    }

    public GameObject GetPlayer()
    {
        return currentPlayer;
    }
}