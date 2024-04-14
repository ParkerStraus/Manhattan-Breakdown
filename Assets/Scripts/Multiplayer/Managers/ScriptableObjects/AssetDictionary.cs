using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AssetDictionary", menuName = "Asset Dictionary")]
public class AssetDictionary : ScriptableObject
{
    public List<UnityEngine.Object> items;
    public Dictionary<string, UnityEngine.Object> dictionary;

    public int GetIndex(UnityEngine.Object value)
    {
        return items.IndexOf(value);
    }

    public object GetValue(int  index)
    {
        return items[index];
    }
}
