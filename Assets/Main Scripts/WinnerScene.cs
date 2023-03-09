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
        audioSource =  GetComponent<AudioSource>();

        List<Transform> roundPlayers = gameData.RoundPlayers;

        if(roundPlayers.Count == 1)
        {
            Transform winner = roundPlayers[0];

            winner.position = spawn.position;
        }
        else //Oh... a tie. Awkward.
        {
            winnerMessage.text = "There were no winners...";
            audioSource.clip = noWinnerTheme;
            blackBackground.GetComponent<Renderer>().enabled = true;
            Destroy(confetti.gameObject);
        }
        audioSource.Play(); //play the audio
    }
}
