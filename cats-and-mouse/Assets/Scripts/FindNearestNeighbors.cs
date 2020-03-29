using System.Collections;
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

        //NeighborsGrid(allWaypoints, suppressLog);
        NeighborsGrid2D();
    }

    // deprecated
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

    // Uses 2D raycasting 
    void NeighborsGrid2D()
    {
        Debug.Log(allWaypoints.Length);
        for (int i = 0; i < allWaypoints.Length; i++)
        {
            ContactFilter2D cf = new ContactFilter2D();
            List<RaycastHit2D> hitH_2D = new List<RaycastHit2D>();
            List<RaycastHit2D> hitV_2D = new List<RaycastHit2D>();
            Vector2 rayH_2D = new Vector2(1, 0);
            Vector2 rayV_2D = new Vector2(0, -1);

            Physics2D.Raycast(allWaypoints[i].transform.position, rayH_2D, cf.NoFilter(), hitH_2D, 1f);
            Physics2D.Raycast(allWaypoints[i].transform.position, rayV_2D, cf.NoFilter(), hitV_2D, 1f);

            hitH_2D.RemoveAll(x => x.transform.name.Equals(allWaypoints[i].name));
            hitV_2D.RemoveAll(x => x.transform.name.Equals(allWaypoints[i].name));

            if (hitH_2D.Count > 0 && hitH_2D[0].transform.CompareTag("Waypoints"))
            {
                Debug.Log(allWaypoints[i]);
                Debug.Log(hitH_2D.Count);
                for (int p = 0; p < hitH_2D.Count; p++)
                {
                    Debug.Log(hitH_2D[p].transform);
                }
                allWaypoints[i].GetComponent<Waypoint>().nearestNeighbors.Add(hitH_2D[0].transform.GetComponent<Waypoint>());
                hitH_2D[0].transform.GetComponent<Waypoint>().nearestNeighbors.Add(allWaypoints[i].GetComponent<Waypoint>());
            }
            if (hitV_2D.Count > 0 && hitV_2D[0].transform.CompareTag("Waypoints"))
            {
                allWaypoints[i].GetComponent<Waypoint>().nearestNeighbors.Add(hitV_2D[0].transform.GetComponent<Waypoint>());
                hitV_2D[0].transform.GetComponent<Waypoint>().nearestNeighbors.Add(allWaypoints[i].GetComponent<Waypoint>());
            }
        }
    }
}
