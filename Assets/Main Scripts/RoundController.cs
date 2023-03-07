using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoundController : MonoBehaviour
{
    [SerializeField] GameObject door; //the door to open.
    [SerializeField] GameObject readyZone; //the door to open.
    private BoxCollider2D readyCollider;

    public bool inProgress = false; //Determine if a round is happening.
    
    private Camera cam;

    private Vector3 lerpStartPos;
    private Vector3 lerpEndPos;
    private float lerpGoalTime;
    private float lerpCurrentTime;

    private float scrollSpeed = 5;
    
    public List<Transform> players = new List<Transform>();
    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        lerpEndPos = cam.transform.position;

        readyCollider = readyZone.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(players.Count > 0 && !inProgress)
        {
            VerifyReadyZone();
        }
        else if(inProgress) //round is happening.
        {
            UpdateCamera();
            //VerifyElimination();
        }
    }

    void UpdateCamera() //Update the camera based on average position of all players.
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

        if(lerpEndPos.y != cam.transform.position.y) //we still need to movement!
        {
            lerpCurrentTime += Time.deltaTime;
            float lerpPerc = Mathf.Clamp01(lerpCurrentTime/lerpGoalTime); //we want to make sure clamp is stuck between 0 and 1 just in case.
            cam.transform.position = Vector3.Lerp(lerpStartPos, lerpEndPos, lerpPerc); //0 means it will be at camera position, 1 will be at goal position.
        }
    }

    float EaseOut(float t) //This is not used at the moment, but it can be used to ease (Use it on the time percentage of lerp)
    {
        return 1 - Mathf.Pow(1 - t, 2);
    }

    void VerifyReadyZone()
    {
        List<Collider2D> playersReady = new List<Collider2D>();
        ContactFilter2D cf = new ContactFilter2D();
        cf.SetLayerMask(LayerMask.GetMask("Player"));

        readyCollider.OverlapCollider(cf, playersReady);

        if(playersReady.Count == players.Count * 2) //It is palyers.Count * 2 because of the feet collider also count as a player lol
        {
             door.GetComponent<Door>().OpenDoors();
             inProgress = true;
        }
    }

    /*void VerifyElimination() //Verify if a player should be eliminated (they are at the bottom of the camera)
    {
        List<Transform> toRemove = new List<Transform>();

        int index = 0;
        while(index < players.Count)
        {
            Transform player = players[index];
            if(cam.WorldToScreenPoint(player.position + new Vector3(0, player.GetComponent<CapsuleCollider2D>().size.y/2)).y < 0) //position from the top of the player collision
            {
                players.RemoveAt(index);
                Destroy(player.gameObject);
            }
            else
                index++;
        }
    }*/

    Vector2 GetAveragePosition() //Get the average position of all players.
    {
        Vector2 avgPosition = Vector2.zero;

        Vector2 strongestPosition = Vector2.zero; // keep track of strongest position, help kill slow players. (Adds another to the average)

        foreach (Transform player in players) 
        {
            Vector2 lastGroundPos = player.GetComponent<Player>().LastGroundedPosition;
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
        players.Add(player.transform);
    }
}
