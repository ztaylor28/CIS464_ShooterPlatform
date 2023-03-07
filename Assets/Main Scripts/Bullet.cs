using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float startTime = 0;
    private GameObject shooter; //To know that we can ignore it.
    private float knockbackMultiplier = 0f;
    private bool inactive = false; //bullet can only affect one person.
    private Vector2 startingKnockback; //Bullet has a slight delay, so keep track of startback in case someone shoots someone point blank.
    private bool ready; //Is the gun ready? (Starts moving)

    public GameObject Shooter { get => shooter; set => shooter = value; }
    public float KnockbackMultiplier { get => knockbackMultiplier; set => knockbackMultiplier = value; }
    public bool Ready { get => ready; set => ready = value; }
    public Vector2 StartingKnockback { get => startingKnockback; set => startingKnockback = value; }

    private Rigidbody2D rb;

    void Start()
    {
        startTime = Time.time;
        rb = transform.GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        if((rb.velocity.magnitude < 0.05 || Time.time - startTime > 25) && Ready)
        {
            Destroy(gameObject); //destroy
        }

        transform.right = rb.velocity.normalized;
    }
    void OnCollisionEnter2D(Collision2D collision) //Hit the bullet BODY. (Weaken the knockbackMultiplier)
    {
        GameObject player = collision.gameObject;
        if(shooter == player){return;} //Bullet hit the shooter, ignore it.

        if(player.tag != "Player") //It hit something else, weaken the knockback.
        {
            knockbackMultiplier /= 2;
        }
    }

    void OnTriggerEnter2D(Collider2D collision) //Hit the TIP of the bullet. (Knockback!)
    {
        Vector2 hitVector = transform.right;
        GameObject player = collision.gameObject;
        if(shooter == player){return;} //Bullet hit the shooter, ignore it.

        if(player.tag == "Player" && !inactive)
        {
            inactive = true;

            //float velStrength = (!ready) ? startingKnockback.magnitude : rb.velocity.magnitude;
            float velStrength = 30;

            player.GetComponent<Player>().Knockback(hitVector * velStrength * knockbackMultiplier);
        }
    }
}
