using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        MusicPlayer.Instance.PlayMusic("Intro");
    }

    void Update()
    {
    }

    public void PlayGame()
    { 
        SceneManager.LoadScene("Tower");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeSettings()
    {
        SceneManager.LoadScene("SettingsMenu");
    }
}
