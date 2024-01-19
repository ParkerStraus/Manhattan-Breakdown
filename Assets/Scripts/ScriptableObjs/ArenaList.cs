using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArenaList", menuName = "Arena List")]
public class ArenaList : ScriptableObject
{
    [SerializeField] private List<string> arenas;

    public string GetScene(int index)
    {
        if(index == -1)
        {
            int i = Random.Range(0, arenas.Count);
            return arenas[i];
        }
        else return arenas[index];
    }
}
