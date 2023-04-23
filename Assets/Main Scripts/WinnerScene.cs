using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WinnerScene : MonoBehaviour //set up the logic needed for winner scene.
{
    [SerializeField] GameData gameData;
    [SerializeField] TMP_Text winnerMessage;
    [SerializeField] Transform winnerPosition;
    [SerializeField] Transform loserPosition;
    [SerializeField] Transform confetti;
    [SerializeField] Transform blackBackground;
    [SerializeField] AudioClip noWinnerTheme;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        PlayerManager.Instance.EnableJoining(true);

        List<Transform> roundPlayers = gameData.RoundPlayers;

        Transform winner = null;
        if(roundPlayers.Count == 1)
        {
            winner = roundPlayers[0];

            int playerID = gameData.GamePlayers.FindIndex(a => a == winner) + 1;
            winnerMessage.text = "Player " + playerID + " is the winner!";

            winner.position = winnerPosition.position;
            MusicPlayer.Instance.PlayMusic("Winner");
        }
        else //Oh... a tie. Awkward.
        {
            winnerMessage.text = "There were no winners...";
            blackBackground.GetComponent<Renderer>().enabled = true;
            Destroy(confetti.gameObject);
            MusicPlayer.Instance.PlayMusic("NoWinner");
        }

        //rest of the players...
        foreach(Transform player in gameData.GamePlayers)
        {
            if(player != winner)
            {
                player.gameObject.SetActive(true); //playable again.
                player.position = loserPosition.position;
            }
        }
    }
}
