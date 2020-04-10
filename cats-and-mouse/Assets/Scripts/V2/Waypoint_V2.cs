using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint_V2 : MonoBehaviour
{
    // connected waypoints
    public List<Waypoint_V2> neighbors;
    public bool walkable;

    [Tooltip("Gizmo color for walkable waypoints (for cats) (deprecated)")]
    [SerializeField] Color walkableColor;
    [Tooltip("Gizmo color for non-walkable waypoints (for cats) (deprecated)")]
    [SerializeField] Color nonWalkableColor;
    [Tooltip("Identifier number for the waypoint (deprecated)")]
    [SerializeField] int id;

    void Start()
    {
        walkable = true;
        Invoke("FindNeighbours", 0f);
        Invoke("CheckWalkableWaypointsForCat", 0.1f);
        Invoke("FindWalkableNeighbors", 0.2f);
    }

    //Finds neighbors by sending rays to surrounding.
    // Only sends 4 rays per waypoint (the other 4 directions are covered by neighboring nodes)
    public void FindNeighbours()
    {
        List<List<RaycastHit2D>> listOfHits = new List<List<RaycastHit2D>>();
        List<Vector2> rayLists = new List<Vector2>()
        {
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, -1)
        };

        for (int i = 0; i < rayLists.Count; i++)
        {
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            Physics2D.Raycast(this.transform.position, rayLists[i], new ContactFilter2D().NoFilter(), hits, Mathf.Sqrt(2));

            // remove all hits (colliders) that are:
            //  - same as this waypoint's collider
            //  - same (x,y) position as this waypoint's collider (sometimes there are mistakenly overlapped tile prefabs that shouldn't exist)
            //  - are not in the waypoint layer
            hits.RemoveAll(x => x.transform.gameObject.GetInstanceID().Equals(this.gameObject.GetInstanceID())
                || Vector2.Distance(x.transform.position, this.transform.position) <= 0.5f
                || x.transform.gameObject.layer != LayerMask.NameToLayer("waypoint"));

            // There should only be one hit remaining after the above RemoveAll.
            // Logs warning if there are more. The registering only takes the first hit.
            if (hits.Count > 1)
                Debug.LogWarning("Hit count greater than 1 while parsing neighbors");

            // register the neighbors if hits exist
            if (hits.Count > 0)
            {
                this.GetComponent<Waypoint_V2>().neighbors.Add(hits[0].transform.GetComponent<Waypoint_V2>());
                hits[0].transform.GetComponent<Waypoint_V2>().neighbors.Add(this.GetComponent<Waypoint_V2>());
            }
        }
    }

    // sets param:walkable to true if walkable for cat, false if not
    // basically checks if the tile is adjacent to 8 floor tiles (center of a 3x3 floor tile set).
    public void CheckWalkableWaypointsForCat()
    {
         walkable &= neighbors.Count >= 8;
    }

    // Reduces neighbors list to those that are only walkable by cats
    // must be called after CheckWalkableWaypointsForCat is called by ALL waypoints.
    public void FindWalkableNeighbors()
    {
        if (!walkable)
        {
            neighbors.Clear();
            this.gameObject.SetActive(false);
        }
        else
            neighbors.RemoveAll(x => !x.walkable);
    }

    private void OnDrawGizmos()
    {
        if (neighbors.Count > 0)
        {
            foreach(Waypoint_V2 wp in neighbors)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(this.transform.position + new Vector3(0, 0, -2),
                    wp.transform.position + new Vector3(0, 0, -2));
            }
        }
    }
}
