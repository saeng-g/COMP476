﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNearestNeighbors : MonoBehaviour
{
    GameObject[] allWaypoints;

    bool suppressLog;

    float agentRadius = 1.5f;

    // Start is called before the first frame update
    void Start() {
        suppressLog = true;

        allWaypoints = GameObject.FindGameObjectsWithTag("Waypoints");
        for (int i = 0; i < allWaypoints.Length; i++) {
            allWaypoints[i].transform.name = ("Waypoint" + (i+1));
        }

        NeighborsGrid(allWaypoints, suppressLog);
    }

    void NeighborsGrid(GameObject[] allWaypoints, bool suppressLog) {
        print(allWaypoints.Length); 

        for (int i = 0; i < allWaypoints.Length; i++) {
            RaycastHit hitH, hitV;

            Vector3 raycastH = new Vector3(1, 0, 0);
            Vector3 raycastV = new Vector3(0, -1, 0);

            if (Physics.Raycast(allWaypoints[i].transform.position, raycastH, out hitH, 1f)) {
                if (hitH.transform.tag == "Waypoints") {
                    allWaypoints[i].GetComponent<Waypoint>().nearestNeighbors.Add(hitH.transform.GetComponent<Waypoint>());
                    hitH.transform.GetComponent<Waypoint>().nearestNeighbors.Add(allWaypoints[i].GetComponent<Waypoint>());
                }
            }
            if (Physics.Raycast(allWaypoints[i].transform.position, raycastV, out hitV, 1f)) {
                if (hitV.transform.tag == "Waypoints") {
                    allWaypoints[i].GetComponent<Waypoint>().nearestNeighbors.Add(hitV.transform.GetComponent<Waypoint>());
                    hitV.transform.GetComponent<Waypoint>().nearestNeighbors.Add(allWaypoints[i].GetComponent<Waypoint>());
                }
            }
        }
    }
}