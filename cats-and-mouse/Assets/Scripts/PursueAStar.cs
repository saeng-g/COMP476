using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementBehavior {
    ARRIVE,
    PURSUE,
    FLEE,
    WANDER
}

public class PursueAStar : MonoBehaviour {

    public Transform target; //other agent to interact with

    public Pathfinding pathfind;
    Waypoint current; //where player is currently standing

    [HideInInspector]
    public float velocity = 5f; //how many units to move per 60 frames

    float previousDistanceFromTarget = -1f;
    float currentDistanceFromTarget;

    public MovementBehavior movement;

    public bool atLeftBoundary, atRightBoundary,
        atTopBoundary, atBottomBoundary = false;

    bool wanderDirection = true; //if true, will assign the wandering agent a new random direction to face

    void Start() {
        //InvokeRepeating("RecalculatePath", 2f, 2f);
    }

    // Update is called once per frame
    void FixedUpdate() {
        Arrive();
    }

    void Arrive() {
        Vector3 generalDirection = new Vector3(target.position.x - transform.position.x,
            0, target.position.z - transform.position.z).normalized;

        List<Waypoint> path = BuildPath(generalDirection);
        if (path != null) {
            pathfind.WalkPath(path);
        }
    }

    void RecalculatePath() { //update the path to follow to target to chase every couple seconds
        pathfind.walking = false;
    }

    void Flee() {
        Vector3 centerOfMass = target.position;

        //For getting general direction to flee in, and also 
        //changing direction if direction points outside the level.
        float xComponent = transform.position.x - centerOfMass.x;
        float zComponent = transform.position.z - centerOfMass.z;
        //cases to mitigate getting stuck near walls
        if (atRightBoundary && xComponent > 0) {
            xComponent = 0;
        }
        if (atLeftBoundary && xComponent < 0) {
            xComponent = 0;
        }
        if (atTopBoundary && zComponent > 0) {
            zComponent = 0;
        }
        if (atBottomBoundary && zComponent < 0) {
            zComponent = 0;
        }
        //cases to mitigate getting stuck in corners
        if (atTopBoundary && atRightBoundary) {
            xComponent = -1;
            zComponent = -1;
        }
        if (atBottomBoundary && atRightBoundary) {
            xComponent = -1;
            zComponent = 1;
        }
        if (atBottomBoundary && atLeftBoundary) {
            xComponent = 1;
            zComponent = 1;
        }
        if (atTopBoundary && atLeftBoundary) {
            xComponent = 1;
            zComponent = -1;
        }

        Vector3 generalDirection = new Vector3(xComponent, 0, zComponent).normalized;

        List<Waypoint> path = BuildPath(generalDirection);
        if (path != null) {
            pathfind.WalkPath(path);
        }
    }

    void Wander() {
        //if using wandering behavior...
    }

    List<Waypoint> BuildPath(Vector3 generalDirection) {
        //print("building path");

        Waypoint targetPoint = null;
        try {
            targetPoint = target.GetComponent<PlayerMovement>().currentWaypointPlayer;
        }
        catch (NullReferenceException e) {
        }

        /*print("current: "+current);
        print("target: "+targetPoint);*/

        //current: current waypoint player is standing on
        if (current != null && targetPoint != null && !pathfind.walking) {
            List<Waypoint> path = pathfind.GetShortestPath(current, targetPoint, pathfind.heuristicToUse);
            return path;
        }
        else {
            return null;
        }
    }

    Waypoint GetCurrentWaypoint(Collider other) {
        current = other.GetComponent<Waypoint>();
        return current;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Waypoints") {
            GetCurrentWaypoint(other);
        }

        //logic for interacting with regions indicating boundaries of the map
        /*
        if (other.tag == "Boundary") {
            if (other.name.Contains("Left")) {
                atLeftBoundary = true;
            }
            if (other.name.Contains("Right")) {
                atRightBoundary = true;
            }
            if (other.name.Contains("Top")) {
                atTopBoundary = true;
            }
            if (other.name.Contains("Bottom")) {
                atBottomBoundary = true;
            }
        }
        */

        //logic for mouse being caught
        if (other.transform.tag == "Mouse") {
            KillPlayer();
        }
    }

    private void OnTriggerExit(Collider other) {
        /*
        if (other.tag == "Boundary") {
            if (other.name.Contains("Left")) {
                atLeftBoundary = false;
            }
            if (other.name.Contains("Right")) {
                atRightBoundary = false;
            }
            if (other.name.Contains("Top")) {
                atTopBoundary = false;
            }
            if (other.name.Contains("Bottom")) {
                atBottomBoundary = false;
            }
        }
        */
    }

    public void KillPlayer() {

    }
}
