using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour {

    [Tooltip("Speed of the character")]
    [SerializeField] float speed;
    [Tooltip("the camera to follow the player")]
    [SerializeField] Transform playerCamera;
    [Tooltip("Set as false if you want the camera to be stationary. (For testing?)")]
    [SerializeField] bool scrollCamera;
    [Tooltip("The light object that works as the players circle of sight")]
    [SerializeField] Transform visionLight;

    //[Tooltip("If using scrollCamera, set to true")]
    //[SerializeField] bool scrollCamera;

    GameObject cornerTilemap; //for tacticalRegionDetection
    Vector2 currentVelocity;
    public bool isMoving;

    // Cheese related fields
    //score depending on amount of cheese eaten
    float score;
    //max cheese consumption
    const float MAXCHEESE = 20;
    //cheese score
    const float CHEESESCORE = 5;

    // Start is called before the first frame update
    void Start() {
        cornerTilemap = null;
        currentVelocity = new Vector2(0.0f, 0.0f);

        if (speed <= 0)
            speed = 0.5f;
        if (playerCamera == null)
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (visionLight == null)
            visionLight = GameObject.FindGameObjectWithTag("VisionLight").transform;
    }

    // Update is called once per frame
    void FixedUpdate() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if ((Mathf.Abs(x) > 0 || Mathf.Abs(y) > 0) && !(Mathf.Abs(x) > 0 && Mathf.Abs(y) > 0))
        { //if moving on one axis at a time...
            //CheckAdjacentTile(new Vector3(x, y, 0).normalized);
            float rotationAngle = (x < 0) ? 180 : 0;
            transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
            isMoving = true;
        }
        else
            isMoving = false;

        currentVelocity = speed * new Vector2(x, y) * Time.fixedDeltaTime;
        currentVelocity = ComputeVelocityWithCheese(currentVelocity);
        transform.position += (Vector3) currentVelocity;
        if (scrollCamera)
            playerCamera.position = transform.position + (Vector3.forward * -10);
    }

    // For pursue
    Vector2 getNextPosition()
    {
        return (Vector2) transform.position + currentVelocity;
    }

    public GameObject getCorner()
    {
        try
        {
            Waypoint_V2 closestWaypoint = Grid_V2.Instance.FindClosestWaypoint(this.transform.position, false);
            TacticalRegion region = closestWaypoint.transform.GetComponentInParent<TacticalRegion>();
            cornerTilemap = region.gameObject;
        }
        catch(Exception e)
        {
            //Debug.Log(e);
            cornerTilemap = null;
        }
        return cornerTilemap;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("Corner"))
        {
            cornerTilemap = collision.gameObject;
        }

        //luca's code
        if (collision.name.Contains("Cheese"))
        {
            Destroy(collision.gameObject);

            //TODO: Determine what we want score to be
            score += CHEESESCORE;
        }
    }

    private Vector2 ComputeVelocityWithCheese(Vector2 velocity)
    {
        float ratio = 1 - (score / MAXCHEESE);
        ratio = Mathf.Clamp(ratio, 0, 0.8f) + 0.2f;
        return velocity * ratio;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name.Contains("Corner"))
        {
            cornerTilemap = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (Grid_V2.Instance == null)
            return;
        Gizmos.color = Color.red;
        Waypoint_V2 toWp = Grid_V2.Instance.FindClosestWaypoint(this.transform.position, false);
        if (toWp != null)
        {
            Gizmos.DrawLine(this.transform.position, toWp.transform.position);
        }
    }
}
