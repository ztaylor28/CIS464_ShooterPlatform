using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WinnerScene : MonoBehaviour //set up the logic needed for winner scene.
{
    [SerializeField] GameData gameData;
    [SerializeField] TMP_Text winnerMessage;
    [SerializeField] Transform spawn;
    [SerializeField] Transform confetti;
    [SerializeField] Transform blackBackground;
    [SerializeField] AudioClip noWinnerTheme;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        List<Transform> roundPlayers = gameData.RoundPlayers;

        if(roundPlayers.Count == 1)
        {
            Transform winner = roundPlayers[0];

            int playerID = gameData.GamePlayers.FindIndex(a => a == winner) + 1;
            winnerMessage.text = "Player " + playerID + " is the winner!";

            winner.position = spawn.position;
            MusicPlayer.Instance.PlayMusic("Winner");
        }
        else //Oh... a tie. Awkward.
        {
            foreach(Transform player in gameData.GamePlayers)
            {
                player.position = spawn.position;
            }

            winnerMessage.text = "There were no winners...";
            blackBackground.GetComponent<Renderer>().enabled = true;
            Destroy(confetti.gameObject);
            MusicPlayer.Instance.PlayMusic("NoWinner");
        }
    }
}
