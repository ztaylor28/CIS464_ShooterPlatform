using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDevDave;

public class GunsDirectionalController : MonoBehaviour {

    public bool autoFire;
    [Space(10)]
    public float deadzone;

    [HideInInspector]
    // Mouse location
    public Vector3 mouseLoc, Origin, Target;  
    [Space]
    GunsDirectionalAnimator GunsAnim;   
    // Direction Angle towards target
    public float targetAngle; 
    // PlayerGraphics GO, empty GO that holds all the player graphics
    public GameObject playerGraphics;
	// Guns
	public RawImage rGunArm, lGunArm;    
	// GunArm Images
	public Texture2D[] rGunArmImages, lGunArmImages;
    public Texture2D rGunRest, lGunRest;
    [HideInInspector]
    // Location data of gunArms
    public Vector2 rOriginalLoc, lOriginalLoc, rCrouchLoc, lCrouchLoc; 
    [Space]
    public RawImage singleGun;
    public Texture2D[] twoHandedPoses, twoHandedFrames;
    public Texture2D[] twoHandedPosesWithSword, twoHandedFramesWithSword;
    // Flipping an image
    Vector3 flipY = new Vector3 (1, -1, 1);
    // Array length-offset to play the right order of animation frames
    int IndexAnimOffset;
    bool pausePose; 
    [HideInInspector]
    public bool withSword;

    void Start () 
    {
        // Fetch GunsAnimator
        GunsAnim = transform.parent.GetComponent<GunsDirectionalAnimator>(); 

        // Stand Locs
        rOriginalLoc = rGunArm.transform.localPosition; 
        lOriginalLoc = lGunArm.transform.localPosition; 

        // Crouch Locs
        rCrouchLoc = rGunArm.transform.localPosition - Vector3.up * 60;
        lCrouchLoc = lGunArm.transform.localPosition - Vector3.up * 60;
    }
	
	// Update is called once per frame
	void Update ()
    {   
        InitialSetup();   

        AimDual();

        AimSingle();
    }

    public void InitialSetup () 
    {
        // Get the mouse location (screen to world)
        mouseLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Get the direction towards the mouse position
        Target = (mouseLoc - Origin);

        // Take out the z
        Target.z = 0;

        // Calculate the direction towards "Target" as an angle
        targetAngle = Mathf.Atan2(Target.y, Target.x) * Mathf.Rad2Deg;
    }

    public void AimDual()
    {      
        if (GunsAnim.aimSystem != GunsDirectionalAnimator.AimSystem.twoHanded) 
        { 
            // Enable both GunArms
            lGunArm.enabled = true;
            rGunArm.enabled = true;

            // Deadzone check
            if (Target.magnitude > deadzone) 
            {            
                // The angle for the rotation of both GunArm objects
                Quaternion aimDir = Quaternion.AngleAxis(targetAngle, Vector3.forward);

                // Apply rotation         
                rGunArm.transform.rotation = lGunArm.transform.rotation = aimDir;

                // See which of the 8 angles we are in for an index
                int indexAngle = Angles.XYTo8Angle(Target.x, Target.y, playerGraphics.transform.rotation);

                // Assign the right images to the GunArms
                rGunArm.texture = rGunArmImages[indexAngle];
                lGunArm.texture = lGunArmImages[indexAngle];

                // Bidirectional Anim
                if (GunsAnim.bidirectionalAim) 
                {
                    // If the cursor is right of the origin (the character)
                    if (mouseLoc.x > Origin.x)
                        {
                            // Assign rest pose image to left gun arm
                            lGunArm.texture = lGunRest;
                            // Reset angle
                            lGunArm.transform.rotation = playerGraphics.transform.rotation;
                        }
                    else
                        {
                            // Assign rest pose image to right gun arm
                            rGunArm.texture = rGunRest;
                            // Reset angle
                            rGunArm.transform.rotation = playerGraphics.transform.rotation;
                        }
                }
            } // Inside deadzone; Guns in rest pose
            else 
            {
                // Rest Images
                rGunArm.texture = rGunRest;
                lGunArm.texture = lGunRest;

                // Reset angle
                rGunArm.transform.rotation = lGunArm.transform.rotation = playerGraphics.transform.rotation;
            }

            // Fix for overlapping gunArm sprites
            if (mouseLoc.x > Origin.x) 
                lGunArm.transform.SetSiblingIndex(2);
            else 
                lGunArm.transform.SetSiblingIndex(1);
        }
        else 
        {
            // disable
            lGunArm.enabled = false;
            rGunArm.enabled = false;
        }
    }

    public void AimSingle() 
    {
        if (GunsAnim.aimSystem == GunsDirectionalAnimator.AimSystem.twoHanded) 
        {    
            // Enable Gun
            singleGun.enabled = true;

            // Assign angle as euler rotation 
            singleGun.transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

            // Deadzone check
            if (Target.magnitude > deadzone) 
            {            
                // See which of the 8 angles we are in for an index
                int indexAngle = Angles.XYTo8Angle(Target.x, Target.y, playerGraphics.transform.rotation);

                // Assign the right images to the GunArms if we are not firing (Coroutine "FireSingleGun")
                if (!pausePose) 
                {
                    if (!withSword)
                        singleGun.texture = twoHandedPoses[indexAngle];
                    else
                        singleGun.texture = twoHandedPosesWithSword[indexAngle];
                }                

                // Mirror the Gun image
                if (Target.x > 0 || indexAngle == 2 || indexAngle == 6) 
                    singleGun.transform.localScale = Vector3.one;
                else 
                    singleGun.transform.localScale = flipY;

                // Frame offset
                IndexAnimOffset = indexAngle * 5;

                // Firing the Gun
                if (autoFire) 
                {
                    if ((Input.GetMouseButton (0) || Input.GetMouseButton (1)) && !pausePose) 
                    {
                        StartCoroutine(FireSingleGun());
                    }
                }
                else 
                {
                    if ((Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1)) && !pausePose) 
                    {
                        StartCoroutine(FireSingleGun());
                    }
                }              
            } // Inside deadzone; Gun in rest pose
            else 
            {
                // Rest Image
                if (!withSword)
                    singleGun.texture = twoHandedPoses[6];
                else
                    singleGun.texture = twoHandedPosesWithSword[6];

                // Rest angle
                singleGun.transform.rotation = Quaternion.Euler (0, 0, 90);
            }
        }
        else 
        {
            // disable
            singleGun.enabled = false;
        }
    }
    public IEnumerator FireSingleGun () 
    {
        pausePose = true;

        for (int i = 0; i < 5; i++)
        {
            if (!withSword)
                singleGun.texture = twoHandedFrames[i + IndexAnimOffset];
            else 
                singleGun.texture = twoHandedFramesWithSword[i + IndexAnimOffset];

            yield return new WaitForSeconds (1 - GunsAnim.animSlider.value);
        }

        pausePose = false;
    }
}