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
    [Tooltip("The light object that works as the players circle of sight")]
    [SerializeField] Transform visionLight;

    //[Tooltip("If using scrollCamera, set to true")]
    //[SerializeField] bool scrollCamera;

    GameObject cornerTilemap; //for tacticalRegionDetection
    Vector2 currentVelocity;
    public bool isMoving;

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
        transform.position += (Vector3) currentVelocity;
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
