using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] float speed;
    [SerializeField] Transform playerCamera;
    [SerializeField] Transform visionLight;

    public bool isMoving;

    [SerializeField] bool scrollCamera;

    bool moving;

    public Waypoint currentWaypointPlayer; //where player currently is

    public Waypoint startingWaypoint; //where player starts
    [HideInInspector]
    public Waypoint currentWaypoint, targetWaypoint;

    private float fraction, journeyLength, startTime; //used to do lerping

    [SerializeField] bool isTrueTopDownSprite;
    GameObject cornerTilemap;

    Vector2 currentVelocity;



    // Start is called before the first frame update
    void Start() {
        cornerTilemap = null;
        currentVelocity = new Vector2(0.0f, 0.0f);
        /*
        if (speed <= 0)
            currentWaypoint = startingWaypoint;
        */

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

        if (!moving) {
            if ((Mathf.Abs(x) > 0 || Mathf.Abs(y) > 0) && !(Mathf.Abs(x) > 0 && Mathf.Abs(y) > 0)) { //if moving on one axis at a time...
                //CheckAdjacentTile(new Vector3(x, y, 0).normalized);
                
                
                if (isTrueTopDownSprite) {
                    float angle = Mathf.Atan2(-x, y) * 180 / Mathf.PI;
                    Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.fixedDeltaTime);
                }
                else {
                    float rotationAngle = (x < 0) ? 180 : 0;
                    transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
                }
                //lerp from current tile to next one
            }
        }
        /*
        else if (targetWaypoint != null) {
            if (fraction < 1) {
                journeyLength = Vector3.Distance(
                    currentWaypoint.transform.position,
                    targetWaypoint.transform.position);

                float distCovered = 0;
                distCovered = (Time.time - startTime) * speed;

                fraction = distCovered / journeyLength;

                transform.position = new Vector3(
                    Vector3.Lerp(
                        currentWaypoint.transform.position,
                        targetWaypoint.transform.position,
                        fraction).x,
                    Vector3.Lerp(
                        currentWaypoint.transform.position,
                        targetWaypoint.transform.position,
                        fraction).y,
                    transform.position.z
                    );
            }
            else {
                currentWaypoint = targetWaypoint; //target point/tile was reached
                moving = false;
            }
        }
        */

        currentVelocity = speed * new Vector2(x, y) * Time.fixedDeltaTime;
        transform.position += (Vector3) currentVelocity;
        playerCamera.position = transform.position + (Vector3.forward * -10);
    }

    
    Waypoint GetCurrentWaypoint(Collider other) {
        currentWaypointPlayer = other.GetComponent<Waypoint>();
        return currentWaypointPlayer;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Waypoints") {
            GetCurrentWaypoint(other);
        }
    }

    void CheckAdjacentTile(Vector3 theDirection) {
        RaycastHit hit;

        if (Physics.Raycast(currentWaypoint.transform.position, theDirection, out hit, 1.5f)) {
            if (hit.transform.tag == "Waypoints") {
                if (hit.transform.GetComponent<Waypoint>().walkable) {
                    //move towards this waypoint
                    targetWaypoint = hit.transform.GetComponent<Waypoint>();
                    fraction = 0;
                    startTime = Time.time;
                    moving = true;
                }
            }
        }
    }

    Vector2 getNextPosition()
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        return currentPos + currentVelocity;
    }

    public GameObject getCorner()
    {
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
}
