using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazards : MonoBehaviour
{
    BoxCollider2D myKillCollider;
    [SerializeField] RoundController roundController; //the door to open.
    [SerializeField] float hazardSpeed; //the door to open.
    [SerializeField] float sawRotateSpeed; //the door to open.
    [SerializeField] Transform targetHazardPosition;

    private GameObject myKillZone;
    private float hazardPace, sawPace;
    // Start is called before the first frame update
    void Start()
    {
        myKillCollider = GetComponent<BoxCollider2D>();
        myKillZone = GameObject.Find("KillZone");
    }

    // Update is called once per frame
    void Update()
    {
        VerifyElimination();
        HazardMovement();
        SpinDemSaws();
    }
    void VerifyElimination() //Verify if a player should be eliminated (they are at the bottom of the camera)
    {
        bool killed = myKillCollider.IsTouchingLayers(LayerMask.GetMask("Player"));

        List<Transform> toRemove = new List<Transform>();

        int index = 0;
        while(index < roundController.players.Count)
        {
            Transform player = roundController.players[index];
            Debug.Log(killed);
            if(killed)
            {
                Debug.Log(killed);
                roundController.players.RemoveAt(index);
                Destroy(player.gameObject);
            }
           else
                index++;
        }
    }

    void HazardMovement()
    {
        hazardPace = hazardSpeed * Time.deltaTime;
        if(roundController.inProgress)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetHazardPosition.position, hazardPace);
        }
    }

    void SpinDemSaws()
    {
        sawPace = sawRotateSpeed * Time.deltaTime;
        Transform[] saws = Tools.GetChildren(transform);

        foreach(Transform saw in saws)
        {
            saw.Rotate(new Vector3( 0, 0, sawPace));
        }
    }
}

