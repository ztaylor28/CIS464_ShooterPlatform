using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
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
    SpriteRenderer spriteRender;
    Transform aimArm;
    private Transform heldItem = null;

    bool isRunning, isRolling, isJumping, isFiring;
    private PlayerInput _controls;

    void Awake()
    {
        _controls = new PlayerInput();
    }
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        spriteRender = GetComponent<SpriteRenderer>();
        aimArm = transform.Find("AimArm");
    }

    
    void Update()
    {
        //FlipSprite(); //Function handles which way the sprite is looking.
        UpdateAnimation();
        UpdateSpeed(); //Handles the stopping of the Player instantly. Allows tight controls.
        isRunning = _controls.Player.Run.ReadValue<float>() > 0; //This checks if key is held or not. (Default: LeftShift)
        isRolling = _controls.Player.Roll.ReadValue<float>() > 0; //This checks if key is held or not. (Default: LeftControl)
        isJumping = _controls.Player.Jump.ReadValue<float>() > 0; //This checks if key is held or not. (Default: Spacebar)
        isFiring = _controls.Player.Fire.ReadValue<float>() > 0; //This checks if key is held or not. (Default: Left Mouse Button)
    }

    private void FixedUpdate() 
    {
        UpdateSpeedFixed(); //This will handle all speed and forces.
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

            //Same local scale as player.
            closestPickUp.localScale = new Vector2 (Mathf.Sign(transform.localScale.x) * Mathf.Abs(closestPickUp.localScale.x), closestPickUp.localScale.y);

            //snap to play
            closestPickUp.rotation = aimArm.Find("Hold").rotation;
            closestPickUp.position = aimArm.Find("Hold").position + (closestPickUp.position - closestPickUp.Find("Grip").transform.position);
            closestPickUp.parent = aimArm.Find("Hold");

            heldItem = closestPickUp;
        }
        else //throw
        {
            heldItem.GetComponent<BoxCollider2D>().enabled = true;
            heldItem.GetComponent<Rigidbody2D>().isKinematic = false;
            heldItem.parent = null; //back to scene

            heldItem.GetComponent<Rigidbody2D>().AddForce(-aimArm.up * 500);

            heldItem = null;
        }
    }

    void OnLook(InputValue value)
    {
        aimInput = value.Get<Vector2>(); //Checks where the player is looking.
        aimInput = Camera.main.ScreenToWorldPoint(aimInput); //Convert input to actual screen position
        
        Vector2 aimVector = (aimInput - (Vector2) aimArm.position);
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

        PlayerWalking(); //Function handles the walk speed.
        PlayerRolling(); //Function handles the hit box for rolling and animation.
        PlayerRunning(); //Function handles the run speed and animation
        PlayerJumping();
        PlayerFiring();
    }

    void UpdateSpeed()
    {
        if(moveInput.x == 0)
        {
            myRigidbody.velocity = new Vector2 (0, myRigidbody.velocity.y); //This code instantly stops the player's X velocity. THIS IMPLEMENTATION MAY NEED TO CHANGE WHEN KNOCKBACK GETS IMPLEMENTED
        }
    }

    void PlayerWalking()
    {
        if(Mathf.Abs(myRigidbody.velocity.x) <= Mathf.Abs(walkSpeed))
        {
            myRigidbody.AddForce(moveInput * walkSpeed * walkSpeed);
        }
    }
    void PlayerRunning()
    {
        if(isRunning)
        {
            if(Mathf.Abs(myRigidbody.velocity.x) <= Mathf.Abs(runSpeed))
            {
                myRigidbody.AddForce(moveInput * runSpeed);
            }
            myAnimator.SetBool("isRunning", true);
        }
        else if (!isRunning)
        {
            myAnimator.SetBool("isRunning", false);
        }
    }

    void PlayerRolling()
    {
        if(isRolling) //If player is holding roll key then change to half hitbox size.
        {
            myBodyCollider.size = new Vector2 (0.7141247f, 0.6f); 
            myBodyCollider.offset = new Vector2 (-0.004566193f, -0.4f);
            myAnimator.SetBool("isRolling", true);
        }
        else if (!isRolling) //If player is not holding roll key change to full hitbox size.
        {
            myAnimator.SetBool("isRolling", false);
            myBodyCollider.size = new Vector2 (0.7141247f, 1.482965f);
            myBodyCollider.offset = new Vector2 (-0.004566193f, -0.06117797f);
        }

        aimArm.GetComponent<SpriteRenderer>().enabled = !isRolling;
        if(heldItem)
            heldItem.GetComponent<SpriteRenderer>().enabled = !isRolling;
    }
    void PlayerJumping()
    {
        if(isJumping) //If player is holding jump key and his feet hitbox is on the ground. Then allow him to jump.
        {
            if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) {return;} 
            myRigidbody.AddForce(new Vector2(1f,jumpHeight), ForceMode2D.Impulse);
        }
        else if (!isJumping) //If player is not holding jump key
        {
            if(Mathf.Sign(myRigidbody.velocity.y) == 1f) //If the player has upward momentum
            {
                myRigidbody.velocity = new Vector2 (myRigidbody.velocity.x, 0f); //Make the momentum negative. THIS IMPLEMENTATION MAY NEED TO CHANGE WHEN KNOCKBACK GETS IMPLEMENTED
            }
        }
    }
    void PlayerFiring()
    {
        if(!heldItem || isRolling) //not holding anything or rolling
            return;

        if(isFiring)
        {
            PickUp pickUp = heldItem.GetComponent<PickUp>();
            pickUp.Fire(transform);
        }
        else if (!isFiring)
        {
            //Placeholder
        }
    }
    void UpdateAnimation()
    {
        bool playerHasBothSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon && Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isJumping", playerHasBothSpeed);

        bool playerHasHorizontalSpeed = (Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon) && (myBodyCollider.size.y > 1);
        myAnimator.SetBool("isWalking", playerHasHorizontalSpeed);

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isJumping", playerHasVerticalSpeed);
    }

    void FlipSprite() //Function Flips the sprite around
    {
        bool playerHasHorizontalSpeed =  Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if(playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void FlipSprite(int sign)
    {
        transform.localScale = new Vector2 (sign, 1f);
    }

    private void OnEnable() //Leave these in, don't know why. Its just in the documentation.
    {
        _controls.Enable();
    }

    private void OnDisable() //Leave these in, don't know why. Its just in the documentation.
    {
        _controls.Disable();
    }
    
}
