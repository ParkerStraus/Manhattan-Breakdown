using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArenaList", menuName = "Arena List")]
public class ArenaList : ScriptableObject
{
    [SerializeField] private List<string> arenas;
    [SerializeField] private Stack<string> arenaInstances = new Stack<string>();

    public string GetScene(int index)
    {
        
        if(index == -1)
        {
            if(arenaInstances.Count == 0)
            {
                string[] tempArenas = arenas.ToArray();
                Shuffle(tempArenas);
                foreach (string arena in tempArenas)
                {
                    arenaInstances.Push(arena);
                }
            }
            return arenaInstances.Pop();
        }
        else return arenas[index];
    }

    static void Shuffle<T>(T[] array)
    {
        System.Random random = new System.Random();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
