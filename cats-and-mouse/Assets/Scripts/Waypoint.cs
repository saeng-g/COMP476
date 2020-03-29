using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    //for each waypoint, it has other waypoints directly connected to it/closest neighbors
    public List<Waypoint> nearestNeighbors;

    public bool visited, walkable, walkableForCat;

	public int id;
    public float heuristic;
    public float distance; //stores its distance from the start point when calculating a shortest path
    public Waypoint previous;

    public Color walk, noWalk, noWalkCat;

    //used for checking adjacent waypoints
    static Vector3 aboveWaypoint = new Vector3(0, 1, 0);
    static Vector3 aboveRightWaypoint = new Vector3(1, 1, 0);
    static Vector3 rightWaypoint = new Vector3(1, 0, 0);
    static Vector3 belowRightWaypoint = new Vector3(1, -1, 0);
    static Vector3 belowWaypoint = new Vector3(0, -1, 0);
    static Vector3 belowLeftWaypoint = new Vector3(-1, -1, 0);
    static Vector3 leftWaypoint = new Vector3(-1, 0, 0);
    static Vector3 aboveLeftWaypoint = new Vector3(-1, 1, 0);

    Vector3[] allAdjacents = {
        aboveWaypoint,
        aboveRightWaypoint,
        rightWaypoint,
        belowRightWaypoint,
        belowWaypoint,
        belowLeftWaypoint,
        leftWaypoint,
        aboveLeftWaypoint };

    void Start() {
        //id = int.Parse(gameObject.name.Replace("Waypoint", ""));

        Invoke("CheckWaypointsPass1", 0.001f); //delay necessary for each step of checking walkability of waypoints
        Invoke("CheckWaypointsPass2", 0.002f);
    }

    void CheckWaypointsPass1() {
        print("Time before waypoints check: " + Time.time);
        //CheckWalkableWaypoints();
    }
    void CheckWaypointsPass2() {
        CheckWalkableWaypointsForCat();
        print("Time after waypoints check: " + Time.time);
    }

    public int GetId() {
		return id;
	}

    public void Highlight(Color highlightColor) {
        GetComponent<SpriteRenderer>().color = highlightColor; //highlights the point itself
    }

    //run this on initialization and any time the level environment changes...
    public void CheckWalkableWaypoints() {
        //check if a wall overlaps this waypoint...
        RaycastHit hit;
        Vector3 raycastBack = new Vector3(0, 0, 1);
        walkable = true;
        Highlight(walk);
        if (Physics.Raycast(transform.position, raycastBack, out hit, 10f)) {
            if (hit.transform.tag == "Wall") {
                walkable = false;
                Highlight(noWalk);
            }
        }
    }

    public void CheckWalkableWaypoints2D()
    {
        //adaptation of CheckWalkableWaypoints for 2D rays and colliders
        Vector2 raycastBack = new Vector2(0.1f, 0.1f);
        walkable = true;
        Highlight(walk);
        if (Physics2D.Raycast(transform.position, raycastBack, 0.1f, LayerMask.GetMask("Wall")))
        {
            Debug.Log("HIT");
            walkable = false;
            Highlight(noWalk);
        }
    }

    //again, run this on initialization and any time the level environment changes...
    public void CheckWalkableWaypointsForCat() {
        if (!walkable) {
            return;
        }
        else {
            walkableForCat = true;
            for (int i = 0; i < allAdjacents.Length; i++) {
                if (walkableForCat) {
                    RaycastHit hit;
                    Vector3 raycast = allAdjacents[i];
                    if (Physics.Raycast(transform.position, raycast, out hit, 1.5f)) {
                        if (hit.transform.GetComponent<Waypoint>() != null && 
                            !hit.transform.GetComponent<Waypoint>().walkable) {
                            walkableForCat = false;
                            Highlight(noWalkCat);
                        }
                    }
                }
            }
        }
    }
}
