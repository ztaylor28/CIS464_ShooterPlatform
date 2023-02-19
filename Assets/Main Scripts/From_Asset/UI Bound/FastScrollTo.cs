using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FastScrollTo : MonoBehaviour
{
    public ScrollRect scroll;

    // void Update () 
    // {
    //     Debug.Log(scroll.verticalNormalizedPosition);
    // } 

    public void ScrollToTop () 
    {
        StartCoroutine(ScrollToPosition(1));
    }
    public void ScrollToBasics () 
    {
        StartCoroutine(ScrollToPosition(0.9995f));
    }
    
    public void ScrollToShooting () 
    {
        StartCoroutine(ScrollToPosition(0.9095f));
    }
    
    public void ScrollToFighting () 
    {
        StartCoroutine(ScrollToPosition(0.8125f));
    }
    
    public void ScrollToSword () 
    {
        StartCoroutine(ScrollToPosition(0.683f));
    }
    
    public void ScrollToDamage () 
    {
        StartCoroutine(ScrollToPosition(0.5915f));
    }
    public void ScrollToClimbing () 
    {
        StartCoroutine(ScrollToPosition(0.4797f));
    }  
    public void ScrollToExtras () 
    {
        StartCoroutine(ScrollToPosition(0.3877f));
    }  
    public IEnumerator ScrollToPosition (float target) 
    {
        float time = 1;

        while (time > 0)
        {
             scroll.verticalNormalizedPosition = Mathf.Lerp (target, scroll.verticalNormalizedPosition, time -= Time.deltaTime);

             if (Input.GetMouseButtonDown(0)) 
                break;

             yield return null;
        }
    }
}
