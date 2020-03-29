using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointDetectorMouse : MonoBehaviour
{
    public Transform      mouse;

    public PlayerMovement movementClass;
    public SphereCollider thisCollider;

    List<Waypoint> nearbyWaypoints = new List<Waypoint>();

    // Start is called before the first frame update
    void Start() {
        thisCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update() {

        Collider[] allColliders = Physics.OverlapSphere(
            new Vector3(mouse.position.x,
                mouse.position.y, 
                -2f), 
            thisCollider.radius);
        nearbyWaypoints.Clear(); //reset list before filling it up

        foreach (Collider c in allColliders) {
            if (c.transform.tag == "Waypoints") {
                nearbyWaypoints.Add(c.GetComponent<Waypoint>());
            }
        }

        float shortestDistance = float.MaxValue;
        for (int i = 0; i < nearbyWaypoints.Count; i++) {
            //nearbyWaypoints[i].Highlight(nearbyWaypoints[i].noWalkCat);
            float distance = Vector3.Distance(
                mouse.position, 
                nearbyWaypoints[i].transform.position);
            if (distance < shortestDistance) {
                shortestDistance = distance;
                movementClass.currentWaypointPlayer = nearbyWaypoints[i];
                //nearbyWaypoints[i].Highlight(nearbyWaypoints[i].walk);
            }

        }
    }
}
