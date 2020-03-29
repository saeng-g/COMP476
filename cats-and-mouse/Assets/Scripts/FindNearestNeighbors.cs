using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNearestNeighbors : MonoBehaviour
{
    GameObject[] allWaypoints;

    public PursueAStar[] allCats;

    // Start is called before the first frame update
    void Start() {
        Invoke("SetupNeighborsGrid", 0.003f);
        Invoke("ReadySignal", 0.004f);
    }

    void SetupNeighborsGrid() {
        allWaypoints = GameObject.FindGameObjectsWithTag("Waypoints");
        for (int i = 0; i < allWaypoints.Length; i++) {
            allWaypoints[i].transform.name = ("Waypoint" + (i + 1));
        }

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

    public void ReadySignal() {
        foreach (PursueAStar cat in allCats) {
            cat.gridReady = true;
        }
    }
}
