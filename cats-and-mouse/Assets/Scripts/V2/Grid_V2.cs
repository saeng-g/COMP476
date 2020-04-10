using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid_V2 : MonoBehaviour
{
    GameObject[] waypoints;

    // Start is called before the first frame update
    void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("Waypoints");
        Debug.Log(waypoints.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
