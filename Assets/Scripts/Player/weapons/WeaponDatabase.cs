using System.Collections.Generic;
using UnityEngine;

public class WeaponDatabase : MonoBehaviour
{
    [SerializeField]
    private List<WeaponData> weapons = new();

    public IReadOnlyList<WeaponData> Weapons => weapons;

    public WeaponData GetRandomWeapon()
    {
        if (weapons.Count == 0)
        {
            return null;
        }

        int index = Random.Range(0, weapons.Count);

        return weapons[index];
    }
}