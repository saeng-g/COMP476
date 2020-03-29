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

    public Material walk, noWalk, noWalkCat;

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
        nearestNeighbors.Clear();
        SetId();

        Invoke("CheckWaypoints", 0.001f); //delay necessary for checking walkability of waypoints
    }

    void CheckWaypoints() {
        CheckWalkableWaypointsForCat();
    }

    public void SetId() {
        id = int.Parse(gameObject.name.Replace("Waypoint", ""));
    }
    public int GetId() {
		return id;
	}

    public void Highlight(Material highlightColor) {
        GetComponent<MeshRenderer>().material = highlightColor; //highlights the point itself
    }

    //again, run this on initialization and any time the level environment changes...
    public void CheckWalkableWaypointsForCat() {
        walkableForCat = true;
        for (int i = 0; i < allAdjacents.Length; i++) {
            RaycastHit hit;
            Vector3 raycast = allAdjacents[i];
            if (Physics.Raycast(transform.position, raycast, out hit, 1.5f)) {
                if (hit.transform.tag == "Wall") {
                    walkableForCat = false;
                    Highlight(noWalkCat);
                }
            }
        }
    }
}
