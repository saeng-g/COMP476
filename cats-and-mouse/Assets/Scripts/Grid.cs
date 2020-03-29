using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool waypointsVisible;
    GameObject[] allWaypoints;

    // Start is called before the first frame update
    void Start() {
        allWaypoints = GameObject.FindGameObjectsWithTag("Waypoints");

        if (!waypointsVisible) {
            for (int i = 0; i < allWaypoints.Length; i++) {
                allWaypoints[i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
