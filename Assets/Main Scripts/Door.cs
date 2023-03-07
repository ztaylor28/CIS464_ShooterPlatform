using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour //The door that blocks the tutorial.
{
    [SerializeField] float openTime;

    public void OpenDoors()
    {
        StartCoroutine(Open());
    }

    IEnumerator Open()
    {
        Transform[] doors = Tools.GetChildren(transform);

        Vector2 startScale = doors[0].localScale;
        Vector2 endScale = new Vector2(0.1f, doors[0].localScale.y);
        float currentTime = 0;

        while(currentTime <= openTime)
        {
            currentTime += Time.deltaTime;
            foreach(Transform door in doors)
            {
                door.localScale = Vector2.Lerp(startScale, endScale, currentTime/openTime);
            }

            yield return new WaitForSeconds(0);
        }
    }
}
