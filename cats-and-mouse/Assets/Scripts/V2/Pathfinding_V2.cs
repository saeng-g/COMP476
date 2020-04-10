using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding_V2 : MonoBehaviour
{
    [Tooltip("Set true to see gizmos showing the fill of the search")]
    [SerializeField] bool showFill;
    [Tooltip("Set true to see gizmos showing the resulting path")]
    [SerializeField] bool showPath;

    private Path p;

    ////////////////////////////////////////////////////////////////////////////////////////
    // Node Class : A single node in the graph
    public class Node
    {
        public Waypoint_V2 wp;
        public float heuristic;

        public Node(Waypoint_V2 wp_)
        {
            this.wp = wp_;
            this.heuristic = 0;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    // Entry Class : A single entry in search algorithm
    // (to be used in the open and closed list of A* Algorithm)
    public class Entry
    {
        public Node node;
        public float fx; //cost so far
        public float ex; //suspected total cost
        public Node from;

        public Entry(Node n, float costSoFar, float heuristicValue, Node fromNode)
        {
            this.node = n;
            this.fx = costSoFar;
            this.ex = costSoFar + heuristicValue;
            this.from = fromNode;
        }
    }

    //helper method for comparing two entries (for ease of sorting lists)
    private int CompareOpenList(Entry x, Entry y)
    {
        return x.ex.CompareTo(y.ex);
    }

    // helper method to debug;
    private void PrintEntries(List<Entry> entries)
    {
        //TODO:
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    // Path Class : A single path
    public class Path
    {
        public List<Entry> pathEntryList = new List<Entry>();
        public List<Entry> trimmedPathEntryList = new List<Entry>();
        public List<Vector2> trimmedPathCoordList = new List<Vector2>();

        public void Clear()
        {
            pathEntryList.Clear();
            trimmedPathEntryList.Clear();
            trimmedPathCoordList.Clear();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    // Node Functions
    // gets the closest node to a particular point
    public Node GetClosestNode(Vector2 point)
    {
        Waypoint_V2 wp = Grid_V2.Instance.FindClosestWaypoint(point);
        return new Node(wp);
    }

    // To be dynamically allocated if we want to develop other heuristic functions
    public delegate float GetHeuristic(Node node, Node goal);

    public float GetEuclidianHeuristics(Node node, Node goal)
    {
        return Vector2.Distance(node.wp.transform.position,
            goal.wp.transform.position);
    }


    ////////////////////////////////////////////////////////////////////////////////////////
    /// PATH FUNCTIONS
    // Determines the shortest path on the grid graph based on start and end positions
    public List<Entry> GetPath(Vector2 startPos, Vector2 endPos, GetHeuristic hx)
    {
        Node startNode = GetClosestNode(startPos);
        Debug.Log("Start:" + startNode.wp);
        Node goalNode = GetClosestNode(endPos);
        Debug.Log("End:" + goalNode.wp);

        List<Entry> openList = new List<Entry>();
        List<Entry> closedList = new List<Entry>();

        // Add starting node into the openList
        Node currentNode = startNode;
        float currentCost = 0;
        Entry currentEntry = new Entry(currentNode, currentCost, hx(startNode, goalNode), null);
        closedList.Add(currentEntry);

        // iterate until GOAL NODE is reached (or fails after too many iters)
        int iter = 0;
        while (!currentNode.wp.Equals(goalNode.wp) && iter < 10000)
        {
            // check each neighbor and add to open list
            foreach (Waypoint_V2 neighbor in currentNode.wp.neighbors)
            {
                if (neighbor == null)
                {
                    Debug.LogError("neighbor was null");
                    Debug.LogError(neighbor.transform.position);
                    Debug.LogError(currentNode.wp.transform.position);
                }
                Node neighborNode = new Node(neighbor);
                if (neighborNode == null || neighborNode.wp == null)
                {
                    Debug.LogError("neighbor node was null");
                    Debug.LogError(neighborNode.wp);
                    Debug.LogError(neighbor.transform.position);
                    Debug.LogError(currentNode.wp.transform.position);
                }
                float cost = Vector3.Distance(currentNode.wp.transform.position, neighbor.transform.position);
                float h = hx(neighborNode, goalNode);
                Entry tmp = new Entry(neighborNode, currentCost + cost, h, currentNode);

                // check if the node has already been visited
                // node is already in openlist
                if (openList.Exists(x => x.node.wp.Equals(neighborNode.wp)))
                {
                    //Debug.Log("NEIGHBOR IN OPEN LIST");
                    Entry k = openList.Find(x => x.node.wp.Equals(neighborNode.wp));
                    if (k.fx > tmp.fx)
                    {
                        openList.Remove(k);
                        openList.Add(tmp);
                    }
                    else continue;
                }
                // node is already in closedlist
                else if (closedList.Exists(x => x.node.wp.Equals(neighborNode.wp)))
                {
                    //Debug.Log("NEIGHBOR IN CLOSED LIST");
                    Entry k = closedList.Find(x => x.node.wp.Equals(neighborNode.wp));
                    if (k.fx > tmp.fx)
                    {
                        closedList.Remove(k);
                        openList.Add(tmp);
                    }
                    else continue;
                }
                else openList.Add(tmp);
            }

            // SORT BASED ON EXPECTED TOTAL COST AND SET CURRENT TO THE LOWEST
            openList.Sort(CompareOpenList);
            currentEntry = openList[0];
            closedList.Add(currentEntry);
            openList.Remove(currentEntry);
            currentNode = currentEntry.node;
            currentCost = currentEntry.fx;
            iter++;
        }
        return closedList;
    }

    // Takes a path (with fill entries) and returns path (without fill entries)
    public List<Entry> TrimPath(List<Entry> path)
    {
        List<Entry> trimmedPath = new List<Entry>();
        Entry currentEntry = path[path.Count - 1];
        trimmedPath.Add(currentEntry);
        for (int i = path.Count - 2; i >= 0; i--)
        {
            Entry tmp = path[i];
            if (tmp.node.wp.Equals(currentEntry.from.wp))
            {
                currentEntry = tmp;
                trimmedPath.Add(currentEntry);
            }
        }
        trimmedPath.Reverse();
        return trimmedPath;
    }

    // Takes a path and returns 2D coordinates of the nodes on the path
    public List<Vector2> CoordFromPath(List<Entry> path)
    {
        List<Vector2> coords = new List<Vector2>();
        foreach (Entry e in path)
        {
            coords.Add(e.node.wp.transform.position);
        }
        return coords;
    }

    // Returns a Path for a given start and end position
    // Path contains fill info, the actual path, and coordinates of the path
    public Path GetPathForChars(Vector2 startPos, Vector2 endPos)
    {
        Path p = new Path();
        p.pathEntryList = GetPath(startPos, endPos, GetEuclidianHeuristics);
        p.trimmedPathEntryList = TrimPath(p.pathEntryList);
        p.trimmedPathCoordList = CoordFromPath(p.trimmedPathEntryList);

        return p;
    }


    ////////////////////////////////////////////////////////////////////////////////////////
    // For visualizing the path finding algos
    public void OnDrawGizmos()
    {
        if (showFill && p != null)
        {
            Gizmos.color = Color.blue;
            foreach (Entry e in p.pathEntryList)
            {
                if (e.from != null && e.node != null)
                {
                    Gizmos.DrawLine(e.from.wp.transform.position, e.node.wp.transform.position);
                }
            }
        }

        if (showPath && p != null)
        {
            Gizmos.color = Color.red;
            foreach (Entry e in p.trimmedPathEntryList)
            {
                if (e.from != null && e.node != null)
                {
                    Gizmos.DrawLine(e.from.wp.transform.position, e.node.wp.transform.position);
                }
            }
        }
    }

    public void Start()
    {
        InvokeRepeating("TestPathGen", 3f, 1f);
    }

    private void TestPathGen()
    {
        Vector2 startpos = new Vector2(Random.Range(-31, 10), Random.Range(-19, 19));
        Vector2 endpos = new Vector2(Random.Range(-31, 10), Random.Range(-19, 19));

        p = GetPathForChars(startpos, endpos);
    }
}
