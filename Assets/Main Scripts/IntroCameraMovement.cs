using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCameraMovement : MonoBehaviour
{
    [SerializeField] float cameraSpeed; 
    [SerializeField] Transform targetCameraPosition;

    float cameraPace;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey) //End the screen prematurely.
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            CameraMovement();
            TargetReached();
        }
    }

    void CameraMovement()
    {
        cameraPace = cameraSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetCameraPosition.position, cameraPace);
    }

    void TargetReached()
    {
        if(transform.position == targetCameraPosition.position)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
