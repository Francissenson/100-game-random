using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class EndRunPortal : MonoBehaviour
{
    private bool playerInside;

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = true;
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

    private void Update()
    {
        if (!playerInside)
        {
            return;
        }

        if (!Input.GetKeyDown(KeyCode.E))
        {
            return;
        }

        if (RunResultManager.Instance != null)
        {
            RunResultManager.Instance.SetVictory();
        }

        SceneManager.LoadScene("EndRunScene");
    }
}