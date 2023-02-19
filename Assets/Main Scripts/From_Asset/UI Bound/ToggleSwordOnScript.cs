using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSwordOnScript : MonoBehaviour
{
    public void Start () 
    {
        ToggleSwordOnAllScripts();
    }
    public void ToggleSwordOnAllScripts () 
    {
        ImageAnimator[] i = GetComponentsInChildren<ImageAnimator>();
        Looking[] j = GetComponentsInChildren<Looking>();
        JumpRoll[] k = GetComponentsInChildren<JumpRoll>();
        Throw[] l = GetComponentsInChildren<Throw>();
        Dash[] o = GetComponentsInChildren<Dash>();
        GunsDirectionalAnimator[] p = GetComponentsInChildren<GunsDirectionalAnimator>();
        GunsDirectionalController[] u = GetComponentsInChildren<GunsDirectionalController>();

        foreach (var item in i) { item.withSword = !item.withSword; }
        foreach (var item in j) { item.withSword = !item.withSword; }
        foreach (var item in k) { item.withSword = !item.withSword; }
        foreach (var item in l) { item.withSword = !item.withSword; }
        foreach (var item in o) { item.withSword = !item.withSword; }
        foreach (var item in p) { item.withSword = !item.withSword; }
        foreach (var item in u) { item.withSword = !item.withSword; }
    }
}