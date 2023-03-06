using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float startTime = 0;
    private GameObject shooter; //To know that we can ignore it.

    public GameObject Shooter { get => shooter; set => shooter = value; }

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
        GameObject player = collision.gameObject;

        if(shooter == player){return;} //Bullet hit the shooter, ignore it.

        if(player.tag == "Player")
        {
            player.GetComponent<PlayerMovement>().Knockback(transform.right * 500);
        }
    }
}
