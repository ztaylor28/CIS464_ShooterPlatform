using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpRoll : MonoBehaviour
{
    [Header("Import Texture2D Images here")]
    public Texture2D[] frames, framesSword;
    private Text title;
    private RawImage imageRenderer;
    private Slider animSlider;
    GameObject image;
    [HideInInspector]
    public bool withSword;

    void Awake ()
    {
        imageRenderer = GetComponentInChildren<RawImage>();
        animSlider = GetComponentInChildren<Slider>();
        image = imageRenderer.gameObject;
    }

    void FixedUpdate () 
    {
        image.transform.rotation *= Quaternion.Euler (0, 0, animSlider.value * -50);

        if (animSlider.value < 0.2f) 
        {
            if (!withSword)
                imageRenderer.texture = frames[0];
            else
                imageRenderer.texture = framesSword[0];
        }
        else 
        {
            if (!withSword)
                imageRenderer.texture = frames[1];
            else
                imageRenderer.texture = framesSword[1];
        }            
    }
}