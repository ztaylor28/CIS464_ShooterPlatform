using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float walkSpeed = 10f;
    [SerializeField] float runSpeed = 30f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float rollDistance = 5f;
    Vector2 moveInput;
    int runInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    bool held = false;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
    }

    
    void Update()
    {
        UpdateSpeed();
        FlipSprite();
        UpdateAnimation();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void OnJump(InputValue value)
    {
        if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) {return;}
        
        if(value.isPressed)
        {
            myRigidbody.velocity += new Vector2 (0f, jumpHeight);
        }
    }


    void OnRoll(InputValue value)
    {
        if(value.isPressed)
        {
            myBodyCollider.size = new Vector2 (0.7141247f, 0.7f);
            myBodyCollider.offset = new Vector2 (-0.004566193f, -0.5f);
            Debug.Log(myBodyCollider.size);
            myRigidbody.velocity += new Vector2 (rollDistance, 0);
        }
    }
    void UpdateSpeed()
    {
        Vector2 playerVelocity = new Vector2 ((moveInput.x * walkSpeed), myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;
    }

    void UpdateAnimation()
    {
        bool playerHasBothSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon && Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isJumping", playerHasBothSpeed);

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isWalking", playerHasHorizontalSpeed);

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isJumping", playerHasVerticalSpeed);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed =  Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if(playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }
}
