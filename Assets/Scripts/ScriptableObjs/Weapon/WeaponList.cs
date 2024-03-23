using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponList", menuName = "Weapon/Weapon List")]
public class WeaponList : ScriptableObject
{
    public List<ScriptableObject> weapons;

    public Weapon GetWeapon(int index)
    {
        if (index == -1)
        {
            return (Weapon)weapons[Random.Range(0, weapons.Count)];
        }
        else
        {
            return (Weapon)weapons[index];
        }
    }

    public int GetWeaponIndex(Weapon weapon)
    {
        return weapons.IndexOf(weapon);
    }


}
