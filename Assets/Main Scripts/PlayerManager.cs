using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour //A singleton class, so only one playermanager can exist.
{
    [SerializeField] GameData gameData;
    private static PlayerManager instance;
    private PlayerInputManager manager;

    public static PlayerManager Instance{ get { return instance; } }

    public PlayerInputManager Manager { get => manager; set => manager = value; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            manager = GetComponent<PlayerInputManager>();
            manager.enabled = true; //Enable the manager. It is disabled on start, as there should only be one PlayerInputManager in the game.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //PLAYER JOINS
    void OnPlayerJoined(PlayerInput player)
    {
        Debug.Log("WHYYYYY");
        gameData.CreatePlayer(player.transform);
        DontDestroyOnLoad(player.gameObject); //We do not want players to get destroyed. Makes resetting scene much easier!
        gameData.RoundPlayers.Add(player.transform); //add player to the round.
    }
}
