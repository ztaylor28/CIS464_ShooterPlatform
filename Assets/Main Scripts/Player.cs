using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float walkSpeed = 10f;
    [SerializeField] float runSpeed = 30f;
    [SerializeField] float jumpHeight = 5f;

    [SerializeField] //float rollDistance = 544f;
    Vector2 moveInput, aimInput;
    int runInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    Transform groundCheck;
    SpriteRenderer spriteRender;
    PlayerInput playerInput;
    Transform aimArm;
    Transform hand;

    private Transform heldItem = null;

    Dictionary<string, bool> booleans = new Dictionary<string, bool>(){
        {"isRunning", false},
        {"isRolling", false},
        {"isJumping", false},
        {"isFiring", false},
        {"jumped", false},
        {"isStunned", false},
        {"rollHeld", false}
    };

    Dictionary<string, bool> defaultBooleans = new Dictionary<string, bool>();

    private float lastJump = 0; //time until last jumped.
    private Vector2 lastGroundedPosition = Vector2.zero; //will be used when calculating average position for camera.
    private float lastGroundedTime = 0;

    private string controlScheme;
    private Gamepad device;

     public Vector2 LastGroundedPosition { get => lastGroundedPosition; set => lastGroundedPosition = value; }
    public string ControlScheme { get => controlScheme; set => controlScheme = value; }
    public Gamepad Device { get => device; set => device = value; }

    void Start()
    {
        Tools.CloneDictionary(booleans, defaultBooleans); //keep track of defautl booleans so we can easily reset them.

        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        //myFeetCollider = GetComponent<BoxCollider2D>();
        spriteRender = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();

        controlScheme = playerInput.currentControlScheme; //Keep track of control scheme, so Unity won't overwrite it.
        if(controlScheme == "Gamepad")
        {
            device = (Gamepad) playerInput.devices[0];
        }

        aimArm = transform.Find("AimArm");
        hand = aimArm.Find("Hold");
        //groundCheck = transform.Find("GroundCheck");
        myFeetCollider = transform.Find("GroundCheck").GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        //FlipSprite(); //Function handles which way the sprite is looking.
        UpdateAnimation();
    }

    void FixedUpdate() 
    {
        UpdateSpeedFixed(); //This will handle all speed and forces.
    }

    void OnEnable() //SetActive(true)
    {
        
    }

    void OnDisable() //Reset all of the booleans and others, so funky stuff won't occur when player respawn
    {
        Tools.CloneDictionary(defaultBooleans, booleans);
        
        if (playerInput.currentControlScheme == "Gamepad") //kill rumble.
        {
            device.SetMotorSpeeds(0f, 0f);
        }

        moveInput = Vector2.zero;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>(); //This just checks if the player is moving at all. This is called by the input controller.
    }

    void OnGrab(InputValue value)
    {
        if(!heldItem) // pick up
        {
            //Get all pickups that are inside the player sprite
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(myBodyCollider.size.x, myBodyCollider.size.y), 0f, LayerMask.GetMask("PickUps"));

            if (colliders.Length < 1) //no items nearby
                return;
            
            //Find the closest pickup to the player.
            Transform closestPickUp = null;
            foreach (Collider2D collider in colliders)
            {
                Transform obj = collider.gameObject.transform;

                if (closestPickUp == null || (obj.position - transform.position).magnitude < (closestPickUp.position - transform.position).magnitude)
                    closestPickUp = obj;
            }

            closestPickUp.GetComponent<BoxCollider2D>().enabled = false; //Gun does not need a hitbox when held.
            closestPickUp.GetComponent<Rigidbody2D>().isKinematic = true; //so gun doesn't have actual physics when held
            closestPickUp.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            //Same local scale as player.
            closestPickUp.localScale = new Vector2 (Mathf.Sign(transform.localScale.x) * Mathf.Abs(closestPickUp.localScale.x), closestPickUp.localScale.y);

            //snap to player
            closestPickUp.rotation = aimArm.Find("Hold").rotation;
            closestPickUp.position = aimArm.Find("Hold").position + (closestPickUp.position - closestPickUp.Find("Grip").transform.position);
            //closestPickUp.parent = aimArm.Find("Hold");

            //We cannot change the parent to the arm anymore due to Players not being able to destroy on load anymore. (This results in the gun to also destroy on load lol)
            ConstraintSource source = new ConstraintSource();
            source.sourceTransform = hand;
            source.weight = 1;
            closestPickUp.GetComponent<ParentConstraint>().AddSource(source);
            closestPickUp.GetComponent<ParentConstraint>().constraintActive = true;

            heldItem = closestPickUp;
        }
        else //throw
        {
            heldItem.GetComponent<BoxCollider2D>().enabled = true;
            heldItem.GetComponent<Rigidbody2D>().isKinematic = false;
            //heldItem.parent = null; //back to scene (Not needed anymore due to using Parent Constraint now, but it was pretty cool to know!)

            heldItem.GetComponent<ParentConstraint>().RemoveSource(0); //remove the source.

            heldItem.GetComponent<Rigidbody2D>().AddForce(-aimArm.up * 500);

            heldItem = null;
        }
    }

    void OnLook(InputValue value)
    {
        aimInput = value.Get<Vector2>(); //Checks where the player is looking.
        
        string TYPE = playerInput.currentControlScheme;

        Vector2 aimVector;
        if(TYPE == "Gamepad")
        {
            aimVector = aimInput;

            if (aimVector.magnitude < 0.25) //Sensitivity
                return;
        }
        else
        {
            aimInput = Camera.main.ScreenToWorldPoint(aimInput); //Convert input to actual screen position
            aimVector = (aimInput - (Vector2) aimArm.position);
        }

        aimArm.up = -aimVector.normalized;

         /* Other way is to calculate the angle, keeping it for notes
            float angle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;
            aimArm.transform.eulerAngles = new Vector3(0,0,angle);
        */

        //Verify if player should flip
        float angle = aimArm.eulerAngles.z;
        if (angle <= 180 &&  angle > 0)
            FlipSprite(1);
        else
            FlipSprite(-1);
    }

    void UpdateSpeedFixed() //This function updates on a fixed timeframe. This is optimal for physics applications.
    {
        //Debug.Log(myRigidbody.velocity.x); //Activate this to check the characters Speed.

        if(booleans["isRunning"])
            PlayerRunning(); //Function handles the run speed and animation
        else
            PlayerWalking(); //Function handles the walk speed.
        

        PlayerRolling(); //Function handles the hit box for rolling and animation.
        PlayerJumping();
        PlayerFiring();
    }

    void PlayerWalking()
    {
        myAnimator.SetBool("isRunning", false);
        ChangeVelocity(new Vector2(moveInput.x * walkSpeed, myRigidbody.velocity.y));
    }
    void PlayerRunning()
    {
        ChangeVelocity(new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y));
        if(!booleans["isRolling"])
        {
            myAnimator.SetBool("isRunning", true);
        }
    }

    void PlayerRolling()
    {
        if(booleans["rollHeld"]) //If player is holding roll key then change to half hitbox size.
        {
            booleans["isRolling"] = true; //player is now rolling.
            myBodyCollider.size = new Vector2 (0.7141247f, 0.6f); 
            myBodyCollider.offset = new Vector2 (-0.004566193f, -0.4f);
            myAnimator.SetBool("isRolling", true);
        }
        else if (!booleans["rollHeld"] && booleans["isRolling"]) //If player is not holding roll key (and rolling) change to full hitbox size.
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector2) transform.position + myBodyCollider.offset, transform.up, 0.4f, LayerMask.GetMask("Ground"));
            
            if(!hit) //No hit, player is not under a block.
            {
                booleans["isRolling"] = false;
                myAnimator.SetBool("isRolling", false);
                myBodyCollider.size = new Vector2 (0.7141247f, 1.482965f);
                myBodyCollider.offset = new Vector2 (-0.004566193f, -0.06117797f);
            }
        }

        aimArm.GetComponent<SpriteRenderer>().enabled = !booleans["isRolling"];
        if(heldItem)
            heldItem.GetComponent<SpriteRenderer>().enabled = !booleans["isRolling"];
    }

    bool CanJump() //can the player jump...?
    {
        bool grounded = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        //ContactFilter2D cf = new ContactFilter2D();
        //cf.SetLayerMask(LayerMask.GetMask("Ground"));
        //bool grounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(0.1f, 0.1f), 0f, cf, new Collider2D[1]) == 1;
        if(!grounded)
        {
            myAnimator.SetBool("isJumping", true);
        }
        else
        {
            myAnimator.SetBool("isJumping", false);
        }

        if(grounded && myRigidbody.velocity.y < 0.01) //player is currently grounded and did NOT jump; update last position and last time.
        {
            if(Time.time - lastJump > 0.1) //so we won't disable a jump that just occurred.
            {
                LastGroundedPosition = transform.position;
                lastGroundedTime = Time.time;
                booleans["jumped"] = false; //player is not jumping.
                return true;
            }
            return false;
        }
        else
        {
            return !booleans["jumped"] && (Time.time - lastGroundedTime) < 0.25; //If the player didn't jump and grounded, they may still jump because of lastGroundedTime.
        }
    }

    void PlayerJumping()
    {
        if(CanJump() && booleans["isJumping"]) //If player is holding jump key and his feet hitbox is on the ground. Then allow him to jump.
        {
            ChangeVelocity(new Vector2(myRigidbody.velocity.x, 0f)); //Reset jump velocity to zero, so coyote jump and have the same jump power as normal.
            myRigidbody.AddForce(new Vector2(1f,jumpHeight * myRigidbody.mass), ForceMode2D.Impulse);
            booleans["jumped"] = true;
            lastJump = Time.time;
        }
        else if (!booleans["isJumping"]) //If player is not holding jump key
        {
            if(Mathf.Sign(myRigidbody.velocity.y) > 0) //If the player has upward momentum, terminate it. (short hop)
            {
                ChangeVelocity(new Vector2(myRigidbody.velocity.x, 0f));
            }
        }
    }
    void PlayerFiring()
    {
        if(!heldItem || booleans["isRolling"]) //not holding anything or rolling
            return;

        if(booleans["isFiring"])
        {
            PickUp pickUp = heldItem.GetComponent<PickUp>();
            pickUp.Fire(transform);
        }
        else if (!booleans["isFiring"])
        {
            //Placeholder
        }
    }

    void UpdateAnimation() //TODO: This should handle all of the animations (rather than it being scattered in different functions everywhere. Worry about that for Project 2.)
    {
        bool playerHasBothSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon && Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        //myAnimator.SetBool("isJumping", playerHasBothSpeed);

        bool playerHasHorizontalSpeed = (Mathf.Abs(myRigidbody.velocity.x) > 0.002) && (myBodyCollider.size.y > 1);
        myAnimator.SetBool("isWalking", playerHasHorizontalSpeed);

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        //myAnimator.SetBool("isJumping", playerHasVerticalSpeed);
        //Debug.Log(myRigidbody.velocity.x);
    }

    void FlipSprite(int sign)
    {
        Tools.FlipSprite(sign, transform);

        if (heldItem) //Also flip the held item. This is needed as the gun is not parented to the player anymore.
            Tools.FlipSprite(sign, heldItem);
    }

    public void ChangeVelocity(Vector2 vector) //This is where the PLAYER script changes the velocity. Uses booleans["isStunned"] to verify if velocity should update or not (means player is being knockbacked.)
    {
        if(!booleans["isStunned"])
        {
            myRigidbody.velocity = vector;
        }
    }

    public void Knockback(Vector2 vector) //knockback the player.
    {
        if(!booleans["isStunned"]) //Player is not stunned already. Reset velocity to make initial impact stronger.
        {
            myRigidbody.velocity = Vector2.zero; //Resets velocity, so knockback can actually feel impactful.
        }
        myRigidbody.AddForce(vector, ForceMode2D.Impulse);

        if(!booleans["isStunned"]) //enable stun
        {
            StartCoroutine(KnockbackDrag());
        }

        if (playerInput.currentControlScheme == "Gamepad")
        {
            StartCoroutine(RumbleController()); //Uses a coroutine as we want rumble to stop after a while
        }
    }

    IEnumerator KnockbackDrag()
    {    
        booleans["isStunned"] = true;
        yield return new WaitForSeconds(0.1f); //Wait a bit. This is so weapons such as Assualt Rifle can still "stun" despite it weak knockback.

                                                 //is player trying to resist?
        while(Mathf.Abs(myRigidbody.velocity.x) > (moveInput.magnitude > 0.01 ? runSpeed : walkSpeed/2) || Mathf.Abs(myRigidbody.velocity.y) > jumpHeight)
        {
            //myRigidbody.velocity += new Vector2 (-Mathf.Sign(myRigidbody.velocity.x) * knockbackDragStrength/100, -Mathf.Sign(myRigidbody.velocity.y) * knockbackDragStrength/100);
            yield return new WaitForSeconds(0);
        }
        booleans["isStunned"] = false;
    }

    IEnumerator RumbleController()
    {
        device.SetMotorSpeeds(0.25f, 0.25f);
         
        // Wait for a short delay
        yield return new WaitForSeconds(0.1f);

        device.SetMotorSpeeds(0f, 0f); //stop rumble
    }
    
    //HELD BUTTONS
    void OnFire(InputValue value){booleans["isFiring"] = value.isPressed;}

    void OnRoll(InputValue value)
    {
        booleans["rollHeld"] = value.isPressed;
    }
    void OnJump(InputValue value){booleans["isJumping"] = value.isPressed;}
    void OnRun(InputValue value){booleans["isRunning"] = value.isPressed;}


/* Possible way to fix PlayerInputMananger; just disable everything except playerInput.
    public void Active(bool isActive)
    {
        foreach(Behaviour comp in GetComponents<Behaviour>())
        {
            if(comp != playerInput)
            {
                comp.enabled = isActive;
            }
            else
            {
   
            }
        }
    }
    */

    //DevConsole
    void OnDevConsole()
    {
        GameObject.Find("Canvas").transform.Find("Console").GetComponent<DevConsole>().Toggle(playerInput); //Toggle the dev console, sending the playerinput so it can disable it.
    }

    void OnPause()
    {
        GameObject.Find("Canvas").transform.Find("PauseMenu").GetComponent<PauseControl>().ChangePause(playerInput); //Toggle the pause menu, sending the playerinput so it can disable it.
    }
}
