using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RoundController : MonoBehaviour
{
    [SerializeField] GameObject door; //the door to open.
    [SerializeField] GameObject readyZone; //the door to open.
    [SerializeField] GameObject buzzSaws;
    [SerializeField] GameData gameData;
    private BoxCollider2D readyCollider;
    private PlayerInputManager playerManager;
    
    private Camera cam;

    private Vector3 lerpStartPos;
    private Vector3 lerpEndPos;
    private float lerpGoalTime;
    private float lerpCurrentTime;

    private float scrollSpeed = 5;
    private bool inProgress = false;
    
    private List<Transform> players;
    
    // Start is called before the first frame update
    void Start()
    {
        MusicPlayer.Instance.PlayMusic("Lobby");

        cam = Camera.main;
        lerpEndPos = cam.transform.position;

        readyCollider = readyZone.GetComponent<BoxCollider2D>();
        playerManager = GetComponent<PlayerInputManager>();

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
        playerManager.EnableJoining(); // Players can join after preexisting players are loaded in.
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
        playerManager.DisableJoining(); //Players cannot join anymore!
        MusicPlayer.Instance.PlayMusic("InProgress");
        door.GetComponent<Door>().OpenDoors();
        buzzSaws.GetComponent<Hazards>().enabled = true; //Start the saw!
        inProgress = true;
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
        if(gameData.GamePlayers.Count == 1) //Playing by themselves and they died, just reset.
        {
            if(players.Count == 0)
            {
                inProgress = false; // round is not in progress anymore.
                SceneManager.LoadScene("Tower"); //reload the scene.
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

    //PLAYER JOINS
    void OnPlayerJoined(PlayerInput player)
    {
        if(!gameData.GamePlayers.Contains(player.transform)) //This is a completely new player. (For some reason, onPlayJoined get called upon new scene. But we can do this check to prevent any conflicts.)
        {
            gameData.CreatePlayer(player.transform);
            DontDestroyOnLoad(player.gameObject); //We do not want players to get destroyed. Makes resetting scene much easier!
            gameData.RoundPlayers.Add(player.transform); //add player to the round.
        }
        else //Already exist. Reset their control scheme in case Unity reset it.
        { //TODO: I hate this bandaid solution. But if it works, it works.
            Player playerScript = player.GetComponent<Player>();
            if (playerScript.ControlScheme == "Gamepad")
            {
                player.SwitchCurrentControlScheme(playerScript.ControlScheme, playerScript.Device);
            }
            else //keyboard and mouse
            {
                player.SwitchCurrentControlScheme(playerScript.ControlScheme, Keyboard.current, Mouse.current);
            }
        }
    }
}
