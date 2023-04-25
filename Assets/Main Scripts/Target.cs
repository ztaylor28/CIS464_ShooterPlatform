using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    ParticleSystem particle;
    bool disabled = false;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void OnTriggerEnter2D(Collider2D collision) //Hit the TIP of the bullet. (Knockback!)/
    {
        if(disabled){return;}
        GameObject obj = collision.gameObject;

        if(obj.tag == "Bullet")
        {
            disabled = true;
            MusicPlayer.PlaySound("TargetBreak", transform);
            Particles.PlayParticle("Burst", transform);
            Destroy(gameObject); //Destroy itself.
        }
    }
}
