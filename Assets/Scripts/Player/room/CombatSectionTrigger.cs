using UnityEngine;

public sealed class CombatSectionTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject sectionToActivate;

    private bool activated;

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if (activated)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        activated = true;

        if (sectionToActivate == null)
        {
            Debug.LogError(
                "[CombatSectionTrigger] Section To Activate missing.");

            return;
        }

        sectionToActivate.SetActive(true);

        gameObject.SetActive(false);
    }
}
