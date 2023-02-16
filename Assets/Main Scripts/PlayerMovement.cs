using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float walkSpeed = 10f;
    [SerializeField] float runSpeed = 30f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float rollDistance = 544f;
    Vector2 moveInput;
    int runInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
    }

    
    void Update()
    {
        FlipSprite();
        UpdateAnimation();
        UpdateSpeed();
    }

    private void FixedUpdate() 
    {
        UpdateSpeedFixed();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) {return;}
        
        if(value.isPressed)
        {
            myRigidbody.AddForce(new Vector2(1f,jumpHeight), ForceMode2D.Impulse);
        }
    }

    void OnRun(InputValue value)
    {
        if(value.isPressed)
        {
            myRigidbody.AddForce(moveInput * runSpeed, ForceMode2D.Impulse);
        }
    }
    void OnRoll(InputValue value)
    {
        if(value.isPressed)
        {
            myBodyCollider.size = new Vector2 (0.7141247f, 0.6f);
            myBodyCollider.offset = new Vector2 (-0.004566193f, -0.4f);
            myRigidbody.AddForce(moveInput * rollDistance, ForceMode2D.Impulse);
        }
    }
    void UpdateSpeedFixed() //This function updates on a fixed timeframe. This is optimal for physics applications.
    {
        if(Mathf.Abs(myRigidbody.velocity.x) <= Mathf.Abs(walkSpeed))
        {
            myRigidbody.AddForce(moveInput * walkSpeed * walkSpeed);
        }
        Debug.Log(myRigidbody.velocity.x);

        if(moveInput.x == 0 && myRigidbody.velocity.y <= 0.5) //This is a horrilbe way to implement this. -Zach (Implemented by Zach)
        {
            myBodyCollider.size = new Vector2 (0.7141247f, 1.482965f);
            myBodyCollider.offset = new Vector2 (-0.004566193f, -0.06117797f);
        }
    }

    void UpdateSpeed()
    {
        if(moveInput.x == 0)
        {
            myRigidbody.velocity = new Vector2 (0, myRigidbody.velocity.y); //This code instantly stops the player's X velocity.
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

        bool playerIsRunning = (Mathf.Abs(myRigidbody.velocity.x) >= (runSpeed+5f));
        myAnimator.SetBool("isRunning", playerIsRunning);

        bool playerIsRollng = myBodyCollider.size.y < 1;
        myAnimator.SetBool("isRolling", playerIsRollng);
    }

    void FlipSprite() //Function Flips the sprite around
    {
        bool playerHasHorizontalSpeed =  Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if(playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }
}
