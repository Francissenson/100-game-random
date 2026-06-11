using UnityEngine;

public sealed class SectionUnlocker : MonoBehaviour
{
    [SerializeField]
    private GameObject gate;

    [SerializeField]
    private GameObject nextTrigger;

    public void UnlockSection()
    {
        if (gate != null)
        {
            gate.SetActive(false);
        }

        if (nextTrigger != null)
        {
            nextTrigger.SetActive(true);
        }

        Debug.Log(
            "[SectionUnlocker] Section Unlocked");
    }
}