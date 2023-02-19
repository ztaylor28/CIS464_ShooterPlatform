using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float startTime = 0;
    
    void Start()
    {
        startTime = Time.time;
    }
    void FixedUpdate()
    {
        if(transform.GetComponent<Rigidbody2D>().velocity.magnitude < 0.05 || Time.time - startTime > 25)
        {
            Destroy(gameObject); //destroy
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject contact = collision.gameObject;

        print("i hit something");
    }
}
