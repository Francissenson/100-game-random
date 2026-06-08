using UnityEngine;

public sealed class RoomExit : MonoBehaviour
{
    [SerializeField]
    private string nextScene;

    private bool playerInside;

    private void Update()
    {
        if (!playerInside)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
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

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = true;

        Debug.Log(
            "[RoomExit] Press E");
    }

    private void OnTriggerExit2D(
        Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = false;
    }
}