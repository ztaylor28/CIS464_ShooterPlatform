using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision) //Hit the TIP of the bullet. (Knockback!)
    {
        GameObject obj = collision.gameObject;

        if(obj.tag == "Bullet")
        {
            Destroy(gameObject); //Destroy itself.
        }
    }
}
