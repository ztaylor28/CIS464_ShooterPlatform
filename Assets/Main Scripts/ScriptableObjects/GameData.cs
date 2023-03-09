using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject //Consists of data relating to the main game.
{
    [System.NonSerialized] List<Transform> gamePlayers = new List<Transform>(); //players who are in the game. Holds PLAYER DATA.
    [System.NonSerialized] List<Transform> roundPlayers = new List<Transform>(); //players who are in the round. Holds PLAYER OBJECTS

    [SerializeField] Color[] colors; //The colors for players.

    public List<Transform> GamePlayers { get => gamePlayers; }
    public List<Transform> RoundPlayers { get => roundPlayers; }

    public void CreatePlayer(Transform player) //Creates a player data.
    {
        player.GetComponent<SpriteRenderer>().color = colors[gamePlayers.Count]; //Assign a color to the player.
        player.Find("AimArm").GetComponent<SpriteRenderer>().color = colors[gamePlayers.Count];
        
        gamePlayers.Add(player);
    }

    public void EliminatePlayer(Transform player)
    {
        roundPlayers.Remove(player);
        player.gameObject.SetActive(false); // "destroy" the player.
    }
}