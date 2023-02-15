using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dash : MonoBehaviour
{
    RawImage imageRenderer;
    [Range (0, 100f)]
    public int ghostAmount;
    [Range (0, 10f)]
    public float fadeSpeed;
    public GameObject ghost, ghostSword;
    public Transform A, B;
    public Texture2D dash, dashSword;
    Transform Target;
    float distance, t;    
    Vector3 flipY = new Vector3 (1, -1, 1);
    [HideInInspector]
    public bool withSword;

    void Start () 
    {
        imageRenderer = GetComponent<RawImage>();
    }    
    public void StartDashTo (string target) 
    {
        if (target == "A") 
            Target = A;

        if (target == "B")
            Target = B;

        // Flip image to keep the up correct
        if (transform.position.x > Target.position.x) 
            transform.localScale = flipY;
        else 
            transform.localScale = Vector3.one;

        // Rotate towards direction of Target 
        var dir = Target.position - transform.position;
        transform.rotation = XLookRotation2D(dir);

        // Distance between this gameobject and Target
        distance = Vector3.Distance (transform.position, Target.position);

        // Reset timer
        t = 0;

        // Start the ghost image spawner coroutine
        StartCoroutine(DoGhostImages());
    }

    void FixedUpdate ()
    {
        if (Target != null) 
        {            
            // Increment for Lerp 
            t += Time.fixedDeltaTime * distance / 3;

            // Move to Target 
            transform.position = Vector3.Lerp (transform.position, Target.position, t);         
        }         

        if (!withSword) 
            imageRenderer.texture = dash;
        else
            imageRenderer.texture = dashSword;
    }

    IEnumerator DoGhostImages () 
    {         
        // Until we achieve close minimal distance, keep instantiating a ghost image every time "i" increments to an odd number
        for (int i = 0; i < ghostAmount; i++)
        {
            if(i % 2 == 1)
            {
                // Declare go for gameObject to save the instantiated object and do something with it
                GameObject go;

                // Instantiate the ghost object
                if (!withSword)
                    go = Instantiate (ghost, transform.position, transform.rotation, transform.parent);
                else
                    go = Instantiate (ghostSword, transform.position, transform.rotation, transform.parent);

                // Also transfer the scale of the original image to the ghost object
                go.transform.localScale = transform.localScale;

                // The sizedelta of the rect transform too (remove if using in an actual game)
                go.GetComponent<RectTransform>().sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;

                // Fading speed of the ghost's images
                go.GetComponent<DashGhost>().speed = fadeSpeed;
            }  

            yield return null;
        }                  
    }

    // credit: https://gamedev.stackexchange.com/questions/139515/lookrotation-make-x-axis-face-the-target-instead-of-z
    Quaternion XLookRotation2D(Vector3 right)
    {
        Quaternion rightToUp = Quaternion.Euler(0f, 0f, 90f);
        Quaternion upToTarget = Quaternion.LookRotation(Vector3.forward, right);

        return upToTarget * rightToUp;
    }
}