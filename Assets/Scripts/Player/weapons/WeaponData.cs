using UnityEngine;

[CreateAssetMenu(
    fileName = "WeaponData",
    menuName = "TopDownShooter/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Info")]
    public string weaponName;

    [TextArea]
    public string description;

    [Header("Weapon")]
    public Weapon weaponPrefab;

    [Header("Visual")]
    public Sprite icon;
}