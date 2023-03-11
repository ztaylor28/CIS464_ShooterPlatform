using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
