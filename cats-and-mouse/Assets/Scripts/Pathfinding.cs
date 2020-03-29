using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public enum HeuristicType {
    NONE,
    EUCLIDEAN,
    CLUSTER
};

public class Pathfinding : MonoBehaviour {

    float gridSpacing = 4f;

    public bool showFill;

    public HeuristicType heuristicToUse;

    public Transform agent;

    public Waypoint[] allPoints;

    List<Waypoint> myPath;

    int startPointId = -1;
    int endPointId   = -1;

    float velocity = 5f; //how many units to move per 60 frames

    [HideInInspector]
    public bool walking;

    int currentWaypoint, nextWaypoint; //for the actual walking along the shortest path
    private float startTime, journeyLength, fraction;

    public Color highlightColor, pathColor, normalColor;

    private void FixedUpdate() {
        //path walking code
        try {
            if (fraction >= 1) {
                if (nextWaypoint < myPath.Count - 1) {
                    startTime = Time.time;
                    currentWaypoint++;
                    nextWaypoint++;
                    journeyLength = Vector3.Distance(
                        myPath[currentWaypoint].transform.position,
                        myPath[nextWaypoint].transform.position);
                }
                else { //end walk
                    walking = false;
                    startPointId = -1;
                    endPointId = -1;
                    if (showFill) {
                        foreach (Waypoint q in myPath) {
                            q.Highlight(normalColor);
                        }
                    }
                }
                fraction = 0;
            }
        }
        catch (NullReferenceException e) {

        }

        if (walking && nextWaypoint < myPath.Count) {
            float distCovered = 0;
            distCovered = (Time.time - startTime) * velocity;

            fraction = distCovered / journeyLength;

            agent.transform.position = Vector3.Lerp(
                myPath[currentWaypoint].transform.position,
                myPath[nextWaypoint].transform.position,
                fraction);
        }
    }

    Waypoint GetClosestWaypointTo(Waypoint w, List<Waypoint> unvisitedPoints) {
        Waypoint closestPoint = null;
        float shortestDistance = float.MaxValue;
        for (int j = 0; j < unvisitedPoints.Count; j++) {
            float distance = unvisitedPoints[j].distance;
            if (distance < shortestDistance) {
                shortestDistance = distance;
                closestPoint = unvisitedPoints[j];
            }
        }
        return closestPoint;
    }
    List<Waypoint> RemoveWaypointFromList(List<Waypoint> unvisited, int id) {
        for (int i = 0; i < unvisited.Count; i++) {
            if (unvisited[i].GetId() == id) {
                unvisited.RemoveAt(i); //remove the specified element from list
            }
        }
        return unvisited; //return this list but with the specified element removed
    }

    public List<Waypoint> GetShortestPath(Waypoint startPoint, Waypoint endPoint, HeuristicType h) {
        if (startPoint == null || endPoint == null)
            return null;
        if (startPoint.transform.position == endPoint.transform.position)
            return null;

        //initialize empty list, to become the path
        List<Waypoint> path = new List<Waypoint>();

        Waypoint currentPoint = startPoint; // <- begin path here

        allPoints = GameObject.FindObjectsOfType<Waypoint>();
        foreach (Waypoint q in allPoints) {
            q.visited = false;
        }

        float[] distances = new float[allPoints.Length];
        Waypoint[] previousPoints = new Waypoint[allPoints.Length];

        List<Waypoint> unvisitedPoints = new List<Waypoint>();
        for (int i = 0; i < allPoints.Length; i++) {
            if (allPoints[i].walkable && allPoints[i].walkableForCat) { //only consider points that cat can traverse!
                if (allPoints[i].gameObject.name == startPoint.gameObject.name) {
                    allPoints[i].distance = 0; //distance between start point and itself is 0
                }
                else {
                    allPoints[i].distance = float.MaxValue; //distance unknown at start for everywhere else
                }
                allPoints[i].previous = null;
                unvisitedPoints.Add(allPoints[i]);
            }
        }

        while (unvisitedPoints.Count > 0) { //while unvisited points list not empty
            Waypoint closest = GetClosestWaypointTo(startPoint, unvisitedPoints);

            //compute heuristic here, if necessary
            float heuristic = 0; //default heuristic input to 0
            if (heuristicToUse == HeuristicType.EUCLIDEAN) {
                float dx = Mathf.Abs(closest.transform.position.x - endPoint.transform.position.x);
                float dz = Mathf.Abs(closest.transform.position.z - endPoint.transform.position.z);
                heuristic = gridSpacing * Mathf.Sqrt(dx * dx + dz * dz); //distance of 4 between each individual tile
            }

            for (int j = 0; j < closest.nearestNeighbors.Count; j++) {
                float alt = closest.distance + Vector3.Distance
                    (closest.transform.position, closest.nearestNeighbors[j].transform.position)
                    + heuristic;
                if (alt < closest.nearestNeighbors[j].distance) {
                    closest.nearestNeighbors[j].distance = alt;
                    closest.nearestNeighbors[j].previous = closest;
                    if (showFill) { //rest of the fill, outside of final path
                        closest.nearestNeighbors[j].transform.GetComponent<Waypoint>().Highlight
                            (highlightColor);
                    }
                }
            }

            unvisitedPoints = RemoveWaypointFromList(unvisitedPoints, closest.GetId());
            closest.visited = true;
        }

        //build path using dijkstra's algorithm information, 
        //walking back from end point to start point
        //therefore, start at end point
        path.Add(endPoint);
        if (showFill)
            endPoint.Highlight(pathColor);

        Waypoint previousPoint = endPoint.previous;
        path.Add(previousPoint);
        if (showFill)
            previousPoint.Highlight(pathColor);

        while (previousPoint != null && (previousPoint.GetId() != startPoint.GetId())) {
            previousPoint = previousPoint.previous;
            path.Add(previousPoint);
            if (showFill)
                previousPoint.Highlight(pathColor);
        }

        //reverse path to have order be from start to end
        path.Reverse();

        return path;
    }

    public void WalkPath(List<Waypoint> path) {
        currentWaypoint = -1; nextWaypoint = 0; //on first update, these will be set to values 0 and 1 respectively
        myPath = path;
        fraction = 1;
        startTime = Time.time;
        walking = true;
    }

    public void MouseClickWaypoints() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 500.0f) && hit.transform.tag == "Waypoints") {
                print("hit a waypoint!");
                if (startPointId == -1) { //if start point not yet chosen by mouse...
                    startPointId = hit.transform.GetComponent<Waypoint>().GetId();
                    if (showFill)
                        hit.transform.GetComponent<Waypoint>().Highlight(highlightColor);
                }
                else {
                    endPointId = hit.transform.GetComponent<Waypoint>().GetId();
                    if (showFill)
                        hit.transform.GetComponent<Waypoint>().Highlight(highlightColor);

                    Waypoint start = GameObject.Find("Waypoint" + startPointId).GetComponent<Waypoint>();
                    Waypoint end = GameObject.Find("Waypoint" + endPointId).GetComponent<Waypoint>();

                    if (startPointId != endPointId) { //if specifying a different start and end point
                        myPath = GetShortestPath(start, end, heuristicToUse);
                        WalkPath(myPath);
                    }
                    else {
                        //reset only the selected end point by mouse click
                        endPointId = -1;
                    }
                }
            }
        }
    }

    public void MouseClickNavMesh() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 500.0f) && hit.transform.tag != "Wall") {
                agent.GetComponent<NavMeshAgent>().destination = hit.point;
            }
        }
    }
}
