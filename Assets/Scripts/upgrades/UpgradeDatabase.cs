using System.Collections.Generic;
using UnityEngine;

public sealed class UpgradeDatabase : MonoBehaviour
{
    [SerializeField]
    private List<UpgradeData> upgrades =
        new List<UpgradeData>();

    public IReadOnlyList<UpgradeData> Upgrades => upgrades;

    private void Awake()
    {
        Debug.Log(
            $"[UpgradeDatabase] Loaded {upgrades.Count} upgrades.");
    }
}