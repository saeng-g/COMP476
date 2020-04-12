using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Is able to be queried for closest waypoints
/// </summary>
public class Grid_V2 : MonoBehaviour
{
    // we only want one grid instance
    public static Grid_V2 Instance;
    List<Waypoint_V2> waypoints;
    bool updated = false;

    // Start is called before the first frame update
    void Start()
    {
        updated = false;
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        waypoints = new List<Waypoint_V2>(FindObjectsOfType<Waypoint_V2>());
        Debug.Log("Total nb of waypoints (before walkability update): " + waypoints.Count);
        Invoke("UpdateWaypointsList", 1.5f); // this is so that the function is called after ALL Waypoint_V2 Invoked methods are finished
    }

    // removes unwalkable waypoints from the waypoints list
    // to be executed after waypoints have computed all the non walkable nodes
    void UpdateWaypointsList()
    {
        waypoints.RemoveAll(x => !x.walkable);
        updated = true;
        Debug.Log("Total nb of waypoints (after walkability update): " + waypoints.Count);
    }

    // Returns waypoint closest to the parameter:position
    public Waypoint_V2 FindClosestWaypoint(Vector2 position, bool walkableOnly)
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        float raycastDistance = walkableOnly ? 5f : 3f;
        Physics2D.CircleCast(position, raycastDistance, Vector2.zero, new ContactFilter2D().NoFilter(), hits);
        hits.RemoveAll(x => x.transform.gameObject.layer != LayerMask.NameToLayer("waypoint"));
        if (walkableOnly)
            hits.RemoveAll(x => !x.transform.GetComponent<Waypoint_V2>().walkable);

        float smallestDistance = float.PositiveInfinity;
        Waypoint_V2 closestHit = null;

        foreach (RaycastHit2D hit in hits)
        {
            float d = Vector2.Distance(hit.transform.position, position);
            if (smallestDistance > d)
            {
                closestHit = hit.transform.GetComponent<Waypoint_V2>();
                smallestDistance = d;
            }
        }

        if (closestHit != null)
            return closestHit;
        else
        {
            Debug.LogWarning("Could not find closest waypoint to the following position: " + position);
            return null;
        }
    }
}
