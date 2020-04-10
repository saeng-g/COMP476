using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    ////////////////////////////////////////////////////////////////////////////////////////
    /// Path attributes
    public List<Pathfinding_V2.Entry> pathEntryList = new List<Pathfinding_V2.Entry>();
    public List<Pathfinding_V2.Entry> trimmedPathEntryList = new List<Pathfinding_V2.Entry>();
    public List<Vector3> trimmedPathCoordList = new List<Vector3>();
    public void Clear()
    {
        pathEntryList.Clear();
        trimmedPathEntryList.Clear();
        trimmedPathCoordList.Clear();
    }
}
