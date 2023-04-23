using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    ParticleSystem particle;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void OnTriggerEnter2D(Collider2D collision) //Hit the TIP of the bullet. (Knockback!)
    {
        GameObject obj = collision.gameObject;

        if(obj.tag == "Bullet")
        {
            MusicPlayer.PlaySound("TargetBreak", transform);
            Particles.PlayParticle("Burst", transform);
            Destroy(gameObject); //Destroy itself.
        }
    }
}
