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
}
