using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    [Tooltip("Reference to all the cat characters in the game")]
    [SerializeField] GameObject[] cats;
    [Tooltip("Reference to the Player character in the game")]
    [SerializeField] PlayerMovement player;
    [Tooltip("The smell alerts received by every cat at every computation")]
    [SerializeField] Stack<Vector2> smellAlerts;
    [Tooltip("The sound alerts received by every cat at every computation")]
    [SerializeField] Stack<Vector2> hearAlerts;
    [Tooltip("The ratio of weight put on smell alerts to weight put on sound alerts. Smell should be weighted less than sound as there is a larger degree of uncertainty")]
    [SerializeField] [Range(0.01f, 1)] float smellToHearRatio;

    public const int SMELL_ALERT = 0;
    public const int HEAR_ALERT = 1;

    [SerializeField] bool playerCornered;

    void Start()
    {
        playerCornered = false;
        smellAlerts = new Stack<Vector2>();
        hearAlerts = new Stack<Vector2>();
    }
    // Update is called once per frame
    void Update()
    {
        CheckPlayer();
    }

    void CheckPlayer()
    {
        CheckPlayerCornered();
    }

    void CheckPlayerCornered()
    {
        GameObject corner = player.getCorner();
        if (corner != null)
        {
            playerCornered = true;
            List<Vector2> tacticalLocations = corner.GetComponent<TacticalRegion>().getTacticalWaypointLocations();
            Dictionary<Vector2, GameObject> catTargetDict = new Dictionary<Vector2, GameObject>();
            Dictionary<Vector2, float> minDistDict = new Dictionary<Vector2, float>();

            foreach (Vector2 loc in tacticalLocations)
            {
                List<float> distances = new List<GameObject>(cats).ConvertAll(x => Vector2.Distance(x.transform.position, loc));
                int minIndex = distances.FindIndex(x => x.Equals(Mathf.Min(distances.ToArray())));
                if (!catTargetDict.ContainsValue(cats[minIndex]))
                {
                    catTargetDict.Add(loc, cats[minIndex]);
                    minDistDict.Add(loc, distances[minIndex]);
                }
            }

            foreach(KeyValuePair<Vector2, GameObject> pair in catTargetDict)
            {
                CatMovementController cmc = pair.Value.GetComponent<CatMovementController>();
                cmc.ForceUpdatePath(pair.Key);
                cmc.ChangeMovementState(MovementBehaviorState.ARRIVE);
            }
        }
        else
        {
            playerCornered = false;
        }
    }

    public void AddAlert(Vector2 alertPos, int smellOrHearAlert)
    {
        if (smellOrHearAlert == SMELL_ALERT)
            //update hear alert stack
            smellAlerts.Push(alertPos);
        else
            //update hear alert stack
            hearAlerts.Push(alertPos);
    }

    public Vector2 CalculateGlobalLocationEstimate()
    {
        Debug.Log("Starting Global Location Estimate Calculation");
        int nbSmellAlerts = smellAlerts.Count;
        Debug.LogFormat("There are {0} nbSmellAlerts", nbSmellAlerts);
        Vector2 averageSmellPosition = Vector2.zero;
        Debug.Log("Initiallized averageSmellPosition: " + averageSmellPosition);

        if (nbSmellAlerts > 0)
        {
            averageSmellPosition = smellAlerts.Pop();
            Debug.LogFormat("Reinitializing averageSmellPosition: {0}", averageSmellPosition);
            while (smellAlerts.Count != 0)
            {
                Debug.LogFormat("Adding {0} alert to average", smellAlerts.Peek());
                Debug.LogFormat("Old average: {0}", averageSmellPosition);
                averageSmellPosition += smellAlerts.Pop();
                Debug.LogFormat("New average: {0}", averageSmellPosition);
            }
            averageSmellPosition /= nbSmellAlerts;

            Debug.LogFormat("Average Post division: {0}", averageSmellPosition);
        }

        int nbHearAlerts = hearAlerts.Count;

        Vector2 averageHearPosition = Vector2.zero;
        if (nbHearAlerts > 0)
        {
            averageHearPosition = hearAlerts.Pop();
            while (hearAlerts.Count != 0)
            {
                averageHearPosition += hearAlerts.Pop();
            }
            averageHearPosition /= nbHearAlerts;
        }

        Vector2 finalEstimate = Vector2.zero;
        if (nbSmellAlerts > 0 && nbHearAlerts > 0)
            finalEstimate = (smellToHearRatio * averageSmellPosition + averageHearPosition) / (smellToHearRatio + 1);
        else if (nbSmellAlerts > 0)
            finalEstimate = averageSmellPosition;
        else if (nbHearAlerts > 0)
            finalEstimate = averageHearPosition;
        return finalEstimate;
    }


    Vector2[] smellAlertsCopy;
    Vector2[] hearAlertsCopy;
    Vector2 gizmoEstimate;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(cats[0].transform.position, cats[1].transform.position);
        Gizmos.DrawLine(cats[1].transform.position, cats[2].transform.position);
        Gizmos.DrawLine(cats[2].transform.position, cats[0].transform.position);

        if (smellAlerts == null || hearAlerts == null)
            return;

        if (smellAlerts.Count > 0)
        {
            smellAlertsCopy = smellAlerts.ToArray();
        }
        if (hearAlerts.Count > 0)
        {
            hearAlertsCopy = hearAlerts.ToArray();
        }

        if (smellAlerts.Count > 0 || hearAlerts.Count > 0)
            gizmoEstimate = CalculateGlobalLocationEstimate();

        if (smellAlertsCopy != null)
        {
            foreach (Vector2 alert in smellAlertsCopy)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(player.transform.position, alert);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(alert, gizmoEstimate);
            }
        }

        if (hearAlertsCopy != null)
        {
            foreach (Vector2 alert in hearAlertsCopy)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(player.transform.position, alert);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(alert, gizmoEstimate);
            }
        }
    }

}
