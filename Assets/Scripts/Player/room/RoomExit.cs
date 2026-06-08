using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class RoomExit : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RoomManager roomManager;

    private bool playerInRange;

    private RunManager runManager;

    private void Awake()
    {
        if (roomManager == null)
        {
            roomManager =
                FindFirstObjectByType<RoomManager>();
        }

        runManager =
            FindFirstObjectByType<RunManager>();
    }

    private void Update()
    {
        if (!playerInRange)
        {
            return;
        }

        if (roomManager == null)
        {
            return;
        }

        if (roomManager.CurrentState != RoomState.Completed)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            LoadNextRoom();
        }
    }

    private void LoadNextRoom()
    {
        if (runManager == null)
        {
            Debug.LogError(
                "[RoomExit] RunManager not found.");

            return;
        }

        if (!runManager.HasNextRoom())
        {
            Debug.Log(
                "[RoomExit] Run Complete.");

            return;
        }

        RoomDefinition nextRoom =
            runManager.GetNextRoom();

        if (nextRoom == null)
        {
            Debug.LogError(
                "[RoomExit] Next room is null.");

            return;
        }

        Debug.Log(
            $"[RoomExit] Loading {nextRoom.sceneName}");

        runManager.AdvanceRoom();

        SceneManager.LoadScene(
            nextRoom.sceneName);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = true;

        if (roomManager != null &&
            roomManager.CurrentState == RoomState.Completed)
        {
            Debug.Log(
                "[RoomExit] Press E to continue.");
        }
        else
        {
            Debug.Log(
                "[RoomExit] Room not completed.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = false;
    }
}