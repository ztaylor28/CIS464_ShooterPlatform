using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject //Consists of data relating to the main game.
{
    [System.NonSerialized] List<Transform> gamePlayers = new List<Transform>(); //players who are in the game. Holds PLAYER DATA.
    [System.NonSerialized] List<Transform> roundPlayers = new List<Transform>(); //players who are in the round. Holds PLAYER OBJECTS

    [SerializeField] Color[] colors; //The colors for players.

    public List<Transform> GamePlayers { get => gamePlayers; }
    public List<Transform> RoundPlayers { get => roundPlayers; }

    public void CreatePlayer(Transform newPlayer) //Creates a player data.
    {
        //Find which color to give to the player.
        List<Color> usedColors = new List<Color>();
        foreach(Transform player in GamePlayers)
        {
            usedColors.Add(player.GetComponent<Player>().Color);
        }

        foreach(Color color in colors)
        {
            if(!usedColors.Contains(color))
            {
                newPlayer.GetComponent<Player>().SetColor(color);
                break;
            }
        }
        
        gamePlayers.Add(newPlayer);
    }

    public void DisconnectPlayer(Transform player)
    {
        gamePlayers.Remove(player);
        roundPlayers.Remove(player);
    }

    public void EliminatePlayer(Transform player)
    {
        MusicPlayer.PlaySound("PlayerDeath", player);
        Particles.PlayParticle("Burst", new Color[]{player.GetComponent<Player>().Color}, player);
        roundPlayers.Remove(player);
        player.gameObject.SetActive(false); //Disable everything in the player.
    }
}