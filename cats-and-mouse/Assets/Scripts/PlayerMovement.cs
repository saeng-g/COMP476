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
    [Tooltip("The animator attached to the character")]
    [SerializeField] Animator animator;

    //[Tooltip("If using scrollCamera, set to true")]
    //[SerializeField] bool scrollCamera;

    GameObject cornerTilemap; //for tacticalRegionDetection
    Vector2 currentVelocity;
    public bool isMoving;

    // Cheese related fields
    //score depending on amount of cheese eaten
    public float score;
    //max cheese consumption
    public const float MAXCHEESE = 100;
    //cheese score
    public const float CHEESESCORE = 5;
    public const float CHEESEBARRELSCORE = 10;

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
        if (animator == null)
            animator = GetComponent<Animator>();
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
            SetToWalk(true);
            isMoving = true;
        }
        else
        {
            SetToWalk(false);
            isMoving = false;
            float rotationAngle = Mathf.Round(transform.rotation.eulerAngles.y / 180) * 180;
            transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
        }

        currentVelocity = speed * new Vector2(x, y) * Time.fixedDeltaTime;
        currentVelocity = ComputeVelocityWithCheese(currentVelocity);
        transform.position += (Vector3) currentVelocity;
        if (scrollCamera)
            playerCamera.position = transform.position + (Vector3.forward * -10);
    }

    // For pursue
    public Vector2 getNextPosition()
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
            SetToHappy(true);
            Destroy(collision.gameObject);

            //TODO: Determine what we want score to be
            score += CHEESESCORE;
        }
    }

    private Vector2 ComputeVelocityWithCheese(Vector2 velocity)
    {
        float ratio = 1 - (score / MAXCHEESE);
        ratio = Mathf.Clamp(ratio, 0, 0.6f) + 0.4f;
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


    //animation related functions
    void SetToWalk(bool yes)
    {
        animator.SetBool("isWalking", yes);
    }

    void SetToHappy(bool yes)
    {
        animator.SetBool("isHappy", yes);
        if (yes)
            Invoke("TimeOutHappiness", 3f);
    }

    void TimeOutHappiness()
    {
        animator.SetBool("isHappy", false);
    }

    void SetToExcited(bool yes)
    {
        animator.SetBool("isExcited", yes);
    }
}
