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
            }
        }

        currentVelocity = speed * new Vector2(x, y) * Time.fixedDeltaTime;
        transform.position += (Vector3) currentVelocity;
        playerCamera.position = transform.position + (Vector3.forward * -10);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Waypoint_V2 toWp = Grid_V2.Instance.FindClosestWaypoint(this.transform.position);
        if (toWp != null)
        {
            Gizmos.DrawLine(this.transform.position, toWp.transform.position);
        }
    }
}
