using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDevDave;

public class Throw : MonoBehaviour
{
    public float deadzone;
    [Header("Import Texture2D Images here")]
    public Texture2D[] frames, framesSword;
    private Text title;
    private RawImage imageRenderer;

    // Rest Image
    public Texture2D[] restPose, restPoseWithSword;
    private Slider animSlider;
    // Flipping the character image
    Vector3 flipX = new Vector3 (-1, 1, 1);
    [HideInInspector]
    public bool withSword;
    public bool InAir;

    void Awake () 
    {
        imageRenderer = GetComponentInChildren<RawImage>();
        animSlider = GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Establish origin
        Vector3 Origin = transform.position + transform.up / 2;

        // Get the mouse location (screen to world)
        Vector3 mouseLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Get the direction towards the mouse position
        Vector3 Target = (mouseLoc - Origin);

        // Take out the z
        Target.z = 0;

        // Continuously loading up the frame loop with the walk animation that has the correct angle
        int indexAngle = Angles.XYTo8Angle(Target.x, Target.y);

        // Mirror the Animation (Except for Top & Bottom angles)
        if (Target.x > 0 || indexAngle == 2 || indexAngle == 6) 
            imageRenderer.transform.localScale = Vector3.one;
        else 
            imageRenderer.transform.localScale = flipX;

        // IndexAnim Local Var
        int frameCount;
        
        // IndexAnim offset
        if (InAir) 
            frameCount = 4;
        else 
            frameCount = 7;

        int IndexAnimOffset = indexAngle * frameCount;

        // Simple Repeater Animator
        int indexAnim = Mathf.FloorToInt(Mathf.Repeat(Time.fixedTime * animSlider.value * 40, frameCount - 0.01f));

        if (!withSword)
            imageRenderer.texture = frames[IndexAnimOffset + indexAnim];
        else
            imageRenderer.texture = framesSword[IndexAnimOffset + indexAnim];

        // Rest pose if we are below the deadzone
        if (Target.magnitude < deadzone) 
        {
            // Simple Pingpong Animator
            int indexAnimRest = Mathf.FloorToInt(Mathf.PingPong(Time.fixedTime * animSlider.value * 10, 1.99f));

            if (!withSword)
                imageRenderer.texture = restPose[indexAnimRest];
            else
                imageRenderer.texture = restPoseWithSword[indexAnimRest];
        }   
    }
}