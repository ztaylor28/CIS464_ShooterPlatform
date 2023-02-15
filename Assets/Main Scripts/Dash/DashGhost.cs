using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashGhost : MonoBehaviour
{
    [HideInInspector]
    public float speed;
    float t;
    IEnumerator Start()
    {
        RawImage raw = GetComponent<RawImage>();

        while (raw.color.a > 0)
        {
            t += Time.deltaTime;

            raw.color = Vector4.Lerp (Color.white, new Color (1, 1, 1, 0), t * speed);
            
            yield return null;
        }

        Destroy (gameObject);
    }
}
