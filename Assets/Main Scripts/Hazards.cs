using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazards : MonoBehaviour
{
    [SerializeField] GameData gameData;

    BoxCollider2D myKillCollider;
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
        HazardMovement();
        SpinDemSaws();
    }

    /* Actually, we can just use the collision "onEnter" event. Should be more performance friendly too.
    void VerifyElimination() //Verify if a player should be eliminated (they are at the bottom of the camera)
    {
        List<Collider2D> playersOuchies = new List<Collider2D>();
        ContactFilter2D cf = new ContactFilter2D();
        cf.SetLayerMask(LayerMask.GetMask("Player"));

        myKillCollider.OverlapCollider(cf, playersOuchies);

        int index = 0;
        while(index < playersOuchies.Count)
        {
            Transform player = playersOuchies[index].GetComponent<Transform>(); //If there is a player here, they already touched it.
            gameData.EliminatePlayer(player);
            index++;
        }
    }
    */

    void HazardMovement()
    {
        hazardPace = hazardSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetHazardPosition.position, hazardPace);
    }

    void SpinDemSaws() //lol
    {
        sawPace = sawRotateSpeed * Time.deltaTime;
        Transform[] saws = Tools.GetChildren(transform);

        foreach(Transform saw in saws)
        {
            saw.Rotate(new Vector3( 0, 0, sawPace));
        }
    }

    void OnTriggerEnter2D(Collider2D collision) //Hit the killzone.
    {
        if(collision.gameObject.tag == "Player") //It's a player!
        {
            gameData.EliminatePlayer(collision.transform);
        }
    }
}

