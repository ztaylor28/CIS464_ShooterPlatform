using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : PickUp
{
    [SerializeField] float cooldown = 1;
    [SerializeField] float shotSpeed = 1;
    [SerializeField] float spread = 0;
    [SerializeField] float recoil = 0; //maybe
    [SerializeField] GameObject bullet = null;
    private float lastShot = 0;

    public Gun() : base("GUN"){} 
    public override void Fire()
    {
        if(Time.time > lastShot) //cooldown
        {
            lastShot = Time.time + cooldown;

            GameObject currentBullet = Instantiate(bullet); //spawn bullet
            Transform barrel = transform.Find("Barrel");

            Vector2 aimVector = barrel.transform.TransformDirection(Vector2.right); //get the local vectorspace
            currentBullet.transform.right = aimVector;
            currentBullet.transform.position = barrel.position;
            currentBullet.GetComponent<Rigidbody2D>().velocity = aimVector * shotSpeed;
        }
    }
}
