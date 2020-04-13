using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalRegion : MonoBehaviour
{
    [Tooltip("The weighpoints to serve as tactical waypoints")]
    [SerializeField] List<Waypoint_V2> tacticalWaypoints;
    [Tooltip("The number of tactical waypoints in the region")]
    [SerializeField] int nbTacticalWaypoints;

    private void Start()
    {
        if (tacticalWaypoints == null || tacticalWaypoints.Count < 1)
        {
            Debug.LogError("no tactical waypoints have been assigned to this tactical region: " + this.gameObject.name);
        }
        else if (tacticalWaypoints.Count != nbTacticalWaypoints)
        {
            Debug.LogError("the specified tactical waypoints does not match the number of waypoints needed for the region: " + this.gameObject.name);
        }
        else
        {
            Debug.LogFormat("{0} waypoints have been registered for the region: {1}", nbTacticalWaypoints, this.gameObject.name);
        }
    }

    // returns coordinates of tactical waypoints
    public List<Vector2> getTacticalWaypointLocations()
    {
        List<Vector2> wpPos = new List<Vector2>();
        foreach (Waypoint_V2 wp in tacticalWaypoints)
        {
            wpPos.Add(wp.transform.position);
        }

        return wpPos;
    }

    // returns nb of tactical waypoints for the region
    public int getNbTactialWaypoints()
    {
        return nbTacticalWaypoints;
    }
}
