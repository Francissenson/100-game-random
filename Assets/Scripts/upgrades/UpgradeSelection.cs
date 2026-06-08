using System.Collections.Generic;
using UnityEngine;

public sealed class UpgradeSelection : MonoBehaviour
{
    [SerializeField]
    private UpgradeDatabase upgradeDatabase;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            DebugGenerateChoices();
        }
    }

    public List<UpgradeData> GenerateChoices(int count)
    {
        List<UpgradeData> results =
            new List<UpgradeData>();

        if (upgradeDatabase == null)
        {
            Debug.LogError(
                "[UpgradeSelection] UpgradeDatabase missing.");

            return results;
        }

        List<UpgradeData> pool =
            new List<UpgradeData>(
                upgradeDatabase.Upgrades);

        while (results.Count < count &&
               pool.Count > 0)
        {
            int index =
                Random.Range(
                    0,
                    pool.Count);

            results.Add(pool[index]);

            pool.RemoveAt(index);
        }

        return results;
    }

    private void DebugGenerateChoices()
    {
        List<UpgradeData> choices =
            GenerateChoices(3);

        Debug.Log(
            "[UpgradeSelection] Generated Upgrade Choices");

        for (int i = 0; i < choices.Count; i++)
        {
            Debug.Log(
                $"Choice {i + 1}: {choices[i].upgradeName}");
        }
    }
}