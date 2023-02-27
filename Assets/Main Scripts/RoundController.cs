using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoundController : MonoBehaviour
{
    private Camera cam;

    private Vector3 lerpStartPos;
    private Vector3 lerpEndPos;
    private float lerpGoalTime;
    private float lerpCurrentTime;

    private float scrollSpeed = 5;
    
    private List<PlayerInput> players = new List<PlayerInput>();
    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        lerpEndPos = cam.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
        VerifyElimination();
    }

    void UpdateCamera()
    {
        if(players.Count > 0)
        {
            Vector2 avgPosition = GetAveragePosition();

            if (avgPosition.y > cam.transform.position.y)
            {
                Vector3 newPos = new Vector3(cam.transform.position.x, avgPosition.y, cam.transform.position.z);
                
                lerpStartPos = cam.transform.position;
                lerpEndPos = newPos;
                lerpGoalTime = (lerpStartPos - lerpEndPos).magnitude/scrollSpeed; //get the time for lerp.
                lerpCurrentTime = 0;
            }
        }

        if(lerpEndPos.y != cam.transform.position.y) //we still need to d movement!
        {
            lerpCurrentTime += Time.deltaTime;
            float lerpPerc = Mathf.Clamp01(lerpCurrentTime/lerpGoalTime); //we want to make sure clamp is stuck between 0 and 1 just in case.
            cam.transform.position = Vector3.Lerp(lerpStartPos, lerpEndPos, lerpPerc); //0 means it will be at camera position, 1 will be at goal position.
        }
    }

    float EaseOut(float t)
    {
        return 1 - Mathf.Pow(1 - t, 2);
    }

    void VerifyElimination() //Verify if a player should be eliminated (they are at the bottom of the camera)
    {
        List<PlayerInput> toRemove = new List<PlayerInput>();


        int index = 0;
        while(index < players.Count)
        {
            PlayerInput player = players[index];
            if(cam.WorldToScreenPoint(player.transform.position + new Vector3(0, player.transform.GetComponent<CapsuleCollider2D>().size.y/2)).y < 0) //position from the top of the player collision
            {
                players.RemoveAt(index);
                Destroy(player.gameObject);
            }
            else
                index++;
        }
    }

    Vector2 GetAveragePosition() //Get the average position of all players.
    {
        Vector2 avgPosition = Vector2.zero;

        Vector2 strongestPosition = Vector2.zero; // keep track of strongest position, help kill slow players. (Adds another to the average)

        foreach (PlayerInput player in players) 
        {
            Vector2 lastGroundPos = player.GetComponent<PlayerMovement>().LastGroundedPosition;
            avgPosition += lastGroundPos;

            if(strongestPosition.y < lastGroundPos.y) 
                strongestPosition = lastGroundPos;
        }

        avgPosition += strongestPosition; //add the strongest to the average
        avgPosition /= (players.Count + 1); // + 1 because of the strongestPosition

        return avgPosition;
    }


    //PLAYER JOINS
    void OnPlayerJoined(PlayerInput player)
    {
        players.Add(player);
    }
}
