using UnityEngine;

public sealed class RoomExit : MonoBehaviour
{
    [SerializeField]
    private string nextScene;

    [Header("Gate")]
    [SerializeField]
    private GameObject closedGate;

    [SerializeField]
    private GameObject openGate;

    [SerializeField]
    private GameObject exitTrigger;

    [SerializeField]
    private bool unlocked;

    public void Unlock()
    {
        if (unlocked)
        {
            return;
        }

        unlocked = true;

        if (closedGate != null)
        {
            closedGate.SetActive(false);
        }

        if (openGate != null)
        {
            openGate.SetActive(true);
        }

        if (exitTrigger != null)
        {
            exitTrigger.SetActive(true);
        }

        Debug.Log(
            "[RoomExit] Unlocked");
    }

    public void LoadNextRoom()
    {
        if (!unlocked)
        {
            return;
        }

        if (SceneLoader.Instance == null)
        {
            Debug.LogError(
                "[RoomExit] SceneLoader missing.");

            return;
        }

        SceneLoader.Instance.LoadScene(
            nextScene);
    }
}