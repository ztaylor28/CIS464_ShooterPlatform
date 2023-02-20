using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public string TYPE;
    public PickUp(string TYPE)
    {
        this.TYPE = TYPE;
    }

    public virtual void Fire(Transform player)
    {
        print("You forgot to overwrite the fire.");
    }
}
