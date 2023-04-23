 using UnityEngine;
 using System.Collections;
 using UnityEngine.InputSystem;
 
 public class PlayerManager : MonoBehaviour //A singleton class to play music.
 {
    [SerializeField] GameData gameData;

    private PlayerInputManager manager;
    private static PlayerManager instance = null;
    public static PlayerManager Instance
    {
        get 
        {
        if(!instance) //No instance... Must've skipped the splash screen (editor). Create a new music player object.
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/PlayerManager")); 
        }

        return instance; 
    }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            manager = GetComponent<PlayerInputManager>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnableJoining(bool boolean)
    {
        if(boolean)
            manager.EnableJoining();
        else
            manager.DisableJoining();
    }

    //PLAYER JOINS
    void OnPlayerJoined(PlayerInput player)
    {
        if(!gameData.GamePlayers.Contains(player.transform)) //This is a completely new player. (For some reason, onPlayJoined get called is player becomes active again. But we can do this check to prevent any conflicts.)
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

    void OnPlayerLeft(PlayerInput player) //SetActive and Destroy set it, so we set a boolean to figure out if player was actually destroyed.
    {
        if(player.GetComponent<Player>().Destroyed) //player was destroyed.
        {
            gameData.DisconnectPlayer(player.transform);
        }
            
    }
 }