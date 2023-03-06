using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : PickUp
{
    [SerializeField] float cooldown = 1;
    [SerializeField] float shotSpeed = 1;
    [SerializeField] float shotDecay = 0; //Randomized shot speed offset. Can affect shotspeed
    [SerializeField] float recoil = 0; //recoil of the gun (knockback shooter)
    [SerializeField] float numBullets = 1;
    [SerializeField] float spread = 0; //The angle for multishot, which is set.
    [SerializeField] float aimDecay = 0; //Randomized angle. Can affect spread.
    [SerializeField] GameObject bullet = null;
    private AudioSource shootAudio;
    private float lastShot = 0;
    private float delayTime = 0.005f; //Waits 0.005 seconds in order to shoot the bullet. This is needed for extreme high shot speeds.

    private Transform grip;
    private Transform barrel;

    public Gun() : base("GUN"){} 

    public void Start()
    {
        grip = transform.Find("Grip");
        barrel = transform.Find("Barrel");
        shootAudio = transform.GetComponent<AudioSource>();
    }
    public override void Fire(Transform player)
    {
        if(Time.time > lastShot && canShoot()) //cooldown
        {
            lastShot = Time.time + cooldown;

            float rotationStart = Mathf.Floor(numBullets/2) * -spread;

            shootAudio.pitch = Random.Range(0.8f, 1.2f);
            shootAudio.Play();

            for(int i = 0; i < numBullets; i++)
            {
                GameObject currentBullet = Instantiate(bullet); //spawn bullet
                currentBullet.GetComponent<Bullet>().Shooter = player.gameObject; //TODO: We can also have currentBullet be a type "Bullet", and have the bullet script handle everything below.
                

                Vector2 aimVector = barrel.transform.TransformDirection(Vector2.right); //get the local vectorspace
                currentBullet.transform.right = aimVector;
                currentBullet.transform.position = barrel.position;
                currentBullet.transform.eulerAngles +=  new Vector3(0,0, Random.Range(-aimDecay, aimDecay) + rotationStart + spread * i);
                //currentBullet.GetComponent<Rigidbody2D>().velocity = aimVector * shotSpeed; | Old method, before multi-bullets
                
                //Rather than doing it instant, we add a delay to make it appear
                //currentBullet.GetComponent<Rigidbody2D>().velocity = currentBullet.transform.right * shotSpeed;

                StartCoroutine(ApplyVelocityDelay(currentBullet));
            }

            //Apply recoil
            if (recoil > 0)
                player.GetComponent<PlayerMovement>().Knockback(-barrel.TransformDirection(Vector2.right) * recoil);
        }
    }

    IEnumerator ApplyVelocityDelay(GameObject bullet)
    {
        // Wait for a short delay
        yield return new WaitForSeconds(delayTime);

        if(bullet)
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * (shotSpeed - Random.Range(-shotDecay, shotDecay));
    }

    public bool canShoot() //Check if the gun is inside of a Ground block. If so, do not shoot.
    {
        Vector2 gripToBarrel = barrel.position - grip.position;
        RaycastHit2D hit = Physics2D.Raycast(grip.position, gripToBarrel.normalized, gripToBarrel.magnitude, LayerMask.GetMask("Ground")); //raycast from grip to barrel, checking if it intersecting ground

        return !hit.collider;
    }
}
