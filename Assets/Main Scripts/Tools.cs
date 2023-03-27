using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static Transform[] GetChildren(Transform parent)
    {
        Transform[] children = new Transform[parent.childCount];

        for(int i = 0; i < parent.childCount; i++)
            children[i] = parent.GetChild(i);

        return children;
    }

    public static void CloneDictionary<TKey, TValue>(Dictionary<TKey, TValue> src, Dictionary<TKey, TValue>  dest)
    {
        dest.Clear(); //clear at the dictionary
        
        foreach (var pair in src)
        {
            dest.Add(pair.Key, pair.Value);
        }
    }

    public static void FlipSprite(int sign, Transform tran)
    {
        tran.localScale = new Vector2 (sign * Mathf.Abs(tran.localScale.x), tran.localScale.y);
    }

    public static int RandomInteger(int min, int max)
    {
        return Random.Range(min,max + 1); //I HATE EXCLUSIVE RANDOM!!!!!!!!!!!!!!!!
    }
}
