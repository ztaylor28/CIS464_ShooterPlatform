using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleporter : MonoBehaviour //Upon collision, teleport to the scene.
{
    [SerializeField] string sceneToLoad;

    void OnTriggerEnter2D(Collider2D collision) //Hit
    {
        if(collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
