using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : PickUp
{
    [SerializeField] float cooldown = 1;
    [SerializeField] float shotSpeed = 1;
    [SerializeField] float recoil = 0; //maybe
    [SerializeField] float numBullets = 1;
    [SerializeField] float spread = 0; //If by itself, will determine a random spread (good for fastfire?) If not, will be used as angle for multishot. NOTE: Best cases are ODD numbers.
    [SerializeField] GameObject bullet = null;
    private float lastShot = 0;
    private float delayTime = 0.005f; //Waits 0.005 seconds in order to shoot the bullet. This is needed for extreme high shot speeds.

    public Gun() : base("GUN"){} 
    public override void Fire()
    {
        if(Time.time > lastShot) //cooldown
        {
            lastShot = Time.time + cooldown;

            float rotationStart = Mathf.Floor(numBullets/2) * -spread;
            for(int i = 0; i < numBullets; i++)
            {
                GameObject currentBullet = Instantiate(bullet); //spawn bullet
                Transform barrel = transform.Find("Barrel");

                Vector2 aimVector = barrel.transform.TransformDirection(Vector2.right); //get the local vectorspace
                currentBullet.transform.right = aimVector;
                currentBullet.transform.position = barrel.position;
                currentBullet.transform.eulerAngles +=  new Vector3(0,0, rotationStart + spread * i);
                //currentBullet.GetComponent<Rigidbody2D>().velocity = aimVector * shotSpeed; | Old method, before multi-bullets
                
                //Rather than doing it instant, we add a delay to make it appear
                //currentBullet.GetComponent<Rigidbody2D>().velocity = currentBullet.transform.right * shotSpeed;

                StartCoroutine(ApplyVelocityDelay(currentBullet));
            }
        }
    }

    IEnumerator ApplyVelocityDelay(GameObject bullet)
    {
        // Wait for a short delay
        yield return new WaitForSeconds(delayTime);

        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * shotSpeed;
    }
}
