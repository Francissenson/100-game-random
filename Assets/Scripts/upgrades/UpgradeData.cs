using System;
using UnityEngine;

[Serializable]
public class UpgradeData
{
    [Header("Info")]
    public string upgradeName;

    [TextArea]
    public string description;

    public Sprite icon;

    [Header("Upgrade")]
    public UpgradeType upgradeType;

    public float value;
}
