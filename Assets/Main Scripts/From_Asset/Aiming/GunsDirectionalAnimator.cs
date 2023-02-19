using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDevDave;

public class GunsDirectionalAnimator : MonoBehaviour
{
    public enum AimSystem {twoHanded, standing, walking, crouching, inAir}
    public AimSystem aimSystem;
    [Space(2)]
    public bool bidirectionalAim;
    [Space(20)]
    // PlayerGraphics GO, empty GO that holds all the player graphics
    public GameObject playerGraphics;

    // Main Image	
    public RawImage characterImage;
    // All animation & pose images
    public Texture2D[] characterFrames, standingFrames, walkFrames, crouchFrames, twoHandedLowerBody;
    public Texture2D[] characterFramesWithSword, standingFramesWithSword, walkFramesWithSword, crouchFramesWithSword;
    public Texture2D inAirFrame, inAirFrameWithSword;

    // Rest Image
    public Texture2D[] restPose, restPoseWithSword;

    public GunsDirectionalController GunsCont;
    public Slider animSlider;

    int indexAnim;
    static float pixelDistance = 6f;

    // Flipping the character image
    Vector3 flipX = new Vector3 (-1, 1, 1);
    [HideInInspector]
    public bool withSword;

    void FixedUpdate()
    {
        switch (aimSystem)
        {
            case AimSystem.twoHanded: TwoHandedFireArm(); break;
            case AimSystem.standing: Standing(); break;
            case AimSystem.walking: Walking(); break;
            case AimSystem.crouching: Crouching(); break;
            case AimSystem.inAir: InAir(); break;
        }        
    }
    void Standing () 
    {
        // Simple Pingpong Animator
        indexAnim = Mathf.FloorToInt(Mathf.PingPong(Time.fixedTime * animSlider.value * 10, 1.99f));

        if (!withSword)
            characterImage.texture = characterFrames[indexAnim];
        else
            characterImage.texture = characterFramesWithSword[indexAnim];

        // Establish origin
        GunsCont.Origin = transform.position + transform.up / 2;

        // Y Placement of GunArms to follow the body's up/down animation
        if (indexAnim < 1) 
        {
            GunsCont.rGunArm.transform.localPosition = GunsCont.rOriginalLoc;
            GunsCont.lGunArm.transform.localPosition = GunsCont.lOriginalLoc;
        }
        else 
        {            
            GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rOriginalLoc.x, GunsCont.rOriginalLoc.y - pixelDistance);
            GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.rOriginalLoc.x, GunsCont.rOriginalLoc.y - pixelDistance);
        }

        // Continuously loading up the frame loop with the idle animation that has the correct angle
        int indexAngle = Angles.XYTo6AngleRotated(GunsCont.Target.x, GunsCont.Target.y, playerGraphics.transform.rotation);
        characterFrames[0] = standingFrames[indexAngle * 2];
        characterFrames[1] = standingFrames[indexAngle * 2 + 1];
        characterFramesWithSword[0] = standingFramesWithSword[indexAngle * 2];
        characterFramesWithSword[1] = standingFramesWithSword[indexAngle * 2 + 1];

        // X Placement of Left GunArm to follow the body 
        if (indexAngle == 3) 
            GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lOriginalLoc.x, GunsCont.lGunArm.transform.localPosition.y + pixelDistance);
        else if ( indexAngle != 0)
            GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lOriginalLoc.x - pixelDistance, GunsCont.lGunArm.transform.localPosition.y);
        else 
            GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lOriginalLoc.x, GunsCont.lGunArm.transform.localPosition.y);

        // X Placement of Right GunArm to follow the body 
        if (indexAngle == 3) 
            GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rOriginalLoc.x, GunsCont.rGunArm.transform.localPosition.y + pixelDistance);
        else if (indexAngle == 1 || indexAngle == 2) 
            GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rOriginalLoc.x - pixelDistance, GunsCont.rGunArm.transform.localPosition.y);
        else 
            GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rOriginalLoc.x, GunsCont.rGunArm.transform.localPosition.y);

        // Rest pose if we are below the deadzone
        if (GunsCont.Target.magnitude < GunsCont.deadzone) 
        {
            characterFrames[0] = restPose[0];
            characterFrames[1] = restPose[1];

            characterFramesWithSword[0] = restPoseWithSword[0];
            characterFramesWithSword[1] = restPoseWithSword[1];

            // Placement of GunArms for rest pose
            GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lOriginalLoc.x - pixelDistance, GunsCont.lGunArm.transform.localPosition.y);
            GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rOriginalLoc.x, GunsCont.rGunArm.transform.localPosition.y);
        }   

        // Reset the playerGraphics node
        playerGraphics.transform.rotation = Quaternion.identity;
    }

    void Walking () 
    {
        // Establish origin
        GunsCont.Origin = transform.position + transform.up / 2;

        // IndexAnim offset
        int IndexAnimOffset = 0;

        // Continuously loading up the frame loop with the walk animation that has the correct angle
        int indexAngle = Angles.XYTo4Angle(GunsCont.Target.x, GunsCont.Target.y); 

        switch (indexAngle)
        {
            case 0:
            IndexAnimOffset = 0;
            break;
            case 1:
            IndexAnimOffset = 8;
            break;
            case 2:
            IndexAnimOffset = 16;
            break;
            case 3:
            IndexAnimOffset = 16;
            break;
        }

        // Simple Repeater Animator
        indexAnim = Mathf.FloorToInt(Mathf.Repeat(Time.fixedTime * animSlider.value * 40, 7.99f));

        if (!withSword)
            characterImage.texture = walkFrames[IndexAnimOffset + indexAnim];
        else
            characterImage.texture = walkFramesWithSword[IndexAnimOffset + indexAnim];

        // Y Placement of GunArms to follow the body's up/down animation
        if (indexAnim != 2 && indexAnim != 3 && indexAnim != 6 && indexAnim != 7) 
        {
            GunsCont.rGunArm.transform.localPosition = GunsCont.rOriginalLoc;
            GunsCont.lGunArm.transform.localPosition = GunsCont.lOriginalLoc;

            if (indexAngle == 2) 
            {
                GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rGunArm.transform.localPosition.x, GunsCont.rOriginalLoc.y + pixelDistance);
                GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lGunArm.transform.localPosition.x, GunsCont.rOriginalLoc.y + pixelDistance);
            }
        }
        else 
        {            
            GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rGunArm.transform.localPosition.x, GunsCont.rOriginalLoc.y - pixelDistance);
            GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lGunArm.transform.localPosition.x, GunsCont.rOriginalLoc.y - pixelDistance);

            if (indexAngle == 2) 
            {
                GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rGunArm.transform.localPosition.x, GunsCont.rOriginalLoc.y);
                GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lGunArm.transform.localPosition.x, GunsCont.rOriginalLoc.y);
            }
        }

        // Left & DownLeft & DownRight
        if (indexAngle == 1 || indexAngle == 2) 
        {
            GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rOriginalLoc.x - pixelDistance, GunsCont.rGunArm.transform.localPosition.y);
            GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lOriginalLoc.x - pixelDistance, GunsCont.lGunArm.transform.localPosition.y);
        }
        
        // Top
        if (indexAngle == 3) 
        {
            GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rOriginalLoc.x, GunsCont.rGunArm.transform.localPosition.y);
        }

        // Rest pose if we are below the deadzone
        if (GunsCont.Target.magnitude < GunsCont.deadzone) 
        {
            characterFrames[0] = restPose[0];
            characterFrames[1] = restPose[1];

            characterFramesWithSword[0] = restPoseWithSword[0];
            characterFramesWithSword[1] = restPoseWithSword[1];

            // Placement of GunArms for rest pose
            GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lOriginalLoc.x, GunsCont.lGunArm.transform.localPosition.y);
        }   

        // Reset the playerGraphics node
        playerGraphics.transform.rotation = Quaternion.identity;
    }

    void InAir () 
    {
        // Establish origin
        GunsCont.Origin = transform.position + transform.up / 2;

        // Default arm positions
        GunsCont.rGunArm.transform.localPosition = GunsCont.rOriginalLoc;
        GunsCont.lGunArm.transform.localPosition = GunsCont.lOriginalLoc;

        // Character image
        if (!withSword)
            characterImage.texture = inAirFrame;
        else 
            characterImage.texture = inAirFrameWithSword;

        // Rest pose if we are below the deadzone
        if (GunsCont.Target.magnitude < GunsCont.deadzone) 
        {
            // Placement of GunArms for rest pose
            GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lOriginalLoc.x - pixelDistance, GunsCont.lGunArm.transform.localPosition.y);
        } 

        // Rotate the playerGraphics node
        playerGraphics.transform.rotation *= Quaternion.Euler (0, 0, animSlider.value * -50);
    }

    void Crouching () 
    {
        // Simple Pingpong Animator
        indexAnim = Mathf.FloorToInt(Mathf.PingPong(Time.fixedTime * animSlider.value * 10, 1.99f));

        if (!withSword)
            characterImage.texture = characterFrames[indexAnim];
        else
            characterImage.texture = characterFramesWithSword[indexAnim];

        // Establish origin
        GunsCont.Origin = transform.position;

        // Y Placement of GunArms to follow the body's up/down animation
        if (indexAnim < 1) 
        {
            GunsCont.rGunArm.transform.localPosition = GunsCont.rCrouchLoc;
            GunsCont.lGunArm.transform.localPosition = GunsCont.lCrouchLoc;
        }
        else 
        {            
            GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rCrouchLoc.x, GunsCont.rCrouchLoc.y - pixelDistance);
            GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lCrouchLoc.x, GunsCont.lCrouchLoc.y - pixelDistance);
        }

        // Continuously loading up the frame loop with the idle animation that has the correct angle
        int indexAngle = Angles.XYTo4Angle(GunsCont.Target.x, GunsCont.Target.y, playerGraphics.transform.rotation);    
            
        characterFrames[0] = crouchFrames[indexAngle * 2];
        characterFrames[1] = crouchFrames[indexAngle * 2 + 1];

        characterFramesWithSword[0] = crouchFramesWithSword[indexAngle * 2];
        characterFramesWithSword[1] = crouchFramesWithSword[indexAngle * 2 + 1];

        // Rest pose if we are below the deadzone
        if (GunsCont.Target.magnitude < GunsCont.deadzone) 
        {
            characterFrames[0] = restPose[2];
            characterFrames[1] = restPose[3];

            characterFramesWithSword[0] = restPoseWithSword[2];
            characterFramesWithSword[1] = restPoseWithSword[3];

            // Placement of GunArms for rest pose
            GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lCrouchLoc.x, GunsCont.lGunArm.transform.localPosition.y);
            GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rCrouchLoc.x, GunsCont.rGunArm.transform.localPosition.y);
        }   
        else
        {
            // X Placement of Left GunArm to follow the body 
            // Looking towards right angle
            if (indexAngle == 0) 
            {   // Body's up/down animation
                if (indexAnim < 1) 
                {
                    GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lCrouchLoc.x + pixelDistance * 2, GunsCont.lCrouchLoc.y + pixelDistance * 2);
                    GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rCrouchLoc.x + pixelDistance, GunsCont.rCrouchLoc.y + pixelDistance * 2);
                }
                else 
                {
                    GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lCrouchLoc.x + pixelDistance * 2, GunsCont.lCrouchLoc.y + pixelDistance);
                    GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rCrouchLoc.x + pixelDistance, GunsCont.rCrouchLoc.y + pixelDistance);
                }            
            } // Looking towards left angle
            else if (indexAngle == 2)
            {   // Body's up/down animation
                if (indexAnim < 1) 
                {
                    GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lCrouchLoc.x - pixelDistance * 2, GunsCont.lCrouchLoc.y + pixelDistance * 3);
                    GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rCrouchLoc.x - pixelDistance * 3, GunsCont.rCrouchLoc.y + pixelDistance * 3);
                }
                else 
                {
                    GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lCrouchLoc.x - pixelDistance * 2, GunsCont.lCrouchLoc.y + pixelDistance * 2);
                    GunsCont.rGunArm.transform.localPosition = new Vector2 (GunsCont.rCrouchLoc.x - pixelDistance * 3, GunsCont.rCrouchLoc.y + pixelDistance * 2);
                }
            }
            else 
            {
                GunsCont.lGunArm.transform.localPosition = new Vector2 (GunsCont.lCrouchLoc.x, GunsCont.lGunArm.transform.localPosition.y);
            }
        }

        // Reset the playerGraphics node
        playerGraphics.transform.rotation = Quaternion.identity;
    }   
    void TwoHandedFireArm () 
    {
        // Establish origin
        GunsCont.Origin = transform.position;

        // Continuously loading up the frame loop with the idle animation that has the correct angle
        int indexAngle = Angles.XYTo4Angle(GunsCont.Target.x, GunsCont.Target.y, playerGraphics.transform.rotation); 
   
        characterImage.texture = twoHandedLowerBody[indexAngle];

        // Mirror the Gun image
        if (GunsCont.Target.x > 0 || indexAngle == 1 || indexAngle == 3) 
            characterImage.transform.localScale = Vector3.one;
        else 
            characterImage.transform.localScale = flipX;

        characterImage.transform.rotation = playerGraphics.transform.rotation;
    }
}