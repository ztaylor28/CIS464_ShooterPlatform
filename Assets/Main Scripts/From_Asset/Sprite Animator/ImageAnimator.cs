using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimator : MonoBehaviour
{
    [Header("Import Texture2D Images here")]
    public Texture2D[] frames, framesSword;
    private Text title;
    private RawImage imageRenderer;
    private RectTransform rect;
    private Slider animSlider;
    [HideInInspector]
    public bool withSword;
    // WithSwordDefinite is turned true if the main animations are sword animations already
    public bool playMainAnimationsOnly;

    void Awake () 
    {
        imageRenderer = GetComponentInChildren<RawImage>();
        rect = imageRenderer.GetComponent<RectTransform>();
        animSlider = GetComponentInChildren<Slider>();
    }

    IEnumerator Start()
    {   
        if (frames.Length > 0) 
        {
            while (this.enabled) 
            {
                for (int i = 0; i < frames.Length; i++)
                {
                    // Input frames with or without sword?
                    if (!withSword || playMainAnimationsOnly)
                    imageRenderer.texture = frames[i];
                    else
                    imageRenderer.texture = framesSword[i];

                    // Feed in slider value for animations speed
                    yield return new WaitForSecondsRealtime(1 - animSlider.value);

                    // Pause if speed is down to zero
                    while (animSlider.value == 0)
                    {
                        yield return null;
                    }
                }
            } 
        }        
    }
}