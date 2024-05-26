using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponList", menuName = "Weapon/Weapon List")]
public class WeaponList : ScriptableObject
{
    public List<ScriptableObject> weapons;

    public Weapon GetWeapon(int index)
    {
        if(index >= weapons.Count || index < 0)
        {
            Weapon wpn0 = (Weapon)weapons[0];
            wpn0.Index = 0;
            return wpn0;
        }
        Weapon wpn = (Weapon)weapons[index];
        wpn.Index = index;
        return wpn;
    }

    public int GetWeaponIndex(Weapon weapon)
    {
        return weapons.IndexOf(weapon);
    }

    public int GetRandomWeaponIndex()
    {
        return Random.Range(0, weapons.Count);
    }

    public int GetIndex(Weapon weapon)
    {
        return weapons.IndexOf(weapon);
    }

}
