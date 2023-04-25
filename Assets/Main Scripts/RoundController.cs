using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class RoundController : MonoBehaviour
{
    [SerializeField] GameObject door; //the door to open.
    [SerializeField] GameObject readyZone; //the door to open.
    [SerializeField] GameObject buzzSaws;
    [SerializeField] Transform targetCounter;

    [SerializeField] Transform lobbyWeapons;
    [SerializeField] GameData gameData;
    private BoxCollider2D readyCollider;
    
    private Camera cam;

    private Vector3 lerpStartPos;
    private Vector3 lerpEndPos;
    private float lerpGoalTime;
    private float lerpCurrentTime;

    private bool isTargetMode = false;

    private float scrollSpeed = 5;
    private bool inProgress = false;
    
    private List<Transform> players;

    private List<GameObject> targets;
    
    // Start is called before the first frame update
    void Start()
    {
        MusicPlayer.Instance.PlayMusic("Lobby");

        cam = Camera.main;
        lerpEndPos = cam.transform.position;

        readyCollider = readyZone.GetComponent<BoxCollider2D>();

        players = gameData.RoundPlayers;
        players.Clear(); //clear the round.


        if(gameData.GamePlayers.Count > 0) // players are already in the game. Load them.
        {
            foreach(Transform player in gameData.GamePlayers)
            {
                player.gameObject.SetActive(true); //playable again.
                player.transform.position = new Vector2(0,0);
                players.Add(player);
            }
        }
        PlayerManager.Instance.EnableJoining(true); // Players can join after preexisting players are loaded in.
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
            VerifyElimination();
            VerifyWinner();

            if(gameData.GamePlayers.Count == 1) // singleplayer
            {
                VerifyTargets();

                //make gun gold (infinite ammo for target mode)
                Transform holdItem = gameData.RoundPlayers[0].GetComponent<Player>().HeldItem;
                if(holdItem)
                    holdItem.GetComponent<Gun>().makeGolden(true);
            }
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

        if(playersReady.Count == players.Count)
        {
            BeginRound();
        }
    }

    void BeginRound()
    {
        PlayerManager.Instance.EnableJoining(false); //Players cannot join anymore!
        MusicPlayer.Instance.PlayMusic("InProgress");
        door.GetComponent<Door>().OpenDoors();
        buzzSaws.GetComponent<Hazards>().enabled = true; //Start the saw!
        inProgress = true;

        if(players.Count == 1) // only one player, spawn some targets.
            TargetMode();
        else
        {
            Destroy(lobbyWeapons.gameObject); //destroy the lobby.

            foreach(Transform player in players)
            {
                player.GetComponent<Player>().DestroyHeldItem(); // in case player is holding an item.
            }
        }
    }

    void TargetMode() //enables TargetMode.
    {
        targetCounter.parent.gameObject.SetActive(true); //enable gui
        targets = new List<GameObject>(); //Set up targets.
        isTargetMode = true; //used to differnate from single player vs multiplayer

        foreach (Transform level in GetComponent<MapController>().Segments)
        {
            level.Find("Targets").gameObject.SetActive(true);

            foreach(Transform target in Tools.GetChildren(level.Find("Targets")))
            {
                targets.Add(target.gameObject); //add to list
            }
        }

        //make the starting weapons golden
        foreach(Transform weapon in Tools.GetChildren(lobbyWeapons))
        {
            weapon.GetComponent<Gun>().makeGolden(true);
        }
    }

    void VerifyTargets() //update GUI, and verify if a target was destroyed
    {
        for(int i = targets.Count - 1; i >= 0; i--)
        {
            if(targets[i] == null)
            {
                targets.RemoveAt(i);
            }
        }
        targetCounter.GetComponent<TMP_Text>().text = targets.Count.ToString() + " REMAINING";
    }

    void VerifyElimination() //Verify if a player should be eliminated (they are at the bottom of the camera)
    {
        List<Transform> toRemove = new List<Transform>();

        int index = 0;
        while(index < players.Count)
        {
            Transform player = players[index];
            if(cam.WorldToScreenPoint(player.position + new Vector3(0, player.GetComponent<CapsuleCollider2D>().size.y/2)).y < 0) //position from the top of the player collision
            {
                gameData.EliminatePlayer(player);
            }
            else
                index++;
        }
    }

    void VerifyWinner()
    {
        if(isTargetMode)
        {
            if(players.Count == 0) //they died
            {
                inProgress = false; // round is not in progress anymore.
                SceneManager.LoadScene("Tower"); //reload the scene.
            }
            else if(targets.Count == 0) //woah they got all the targets!
            {
                SceneManager.LoadScene("Winner");
            }
        }
        else if(players.Count <= 1) //Normal game and only one player (or zero if they all died.)
        {
            inProgress = false; // round is not in progress anymore.

            SceneManager.LoadScene("Winner");
        }
    }

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
}
